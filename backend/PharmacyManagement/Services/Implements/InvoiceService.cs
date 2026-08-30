using PharmacyManagement.DTOs.Invoice;
using PharmacyManagement.DTOs.InvoiceItem;
using PharmacyManagement.DTOs.Batch;
using PharmacyManagement.DTOs.UserByBranch;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Extensions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Services.BatchSelection;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;
using backgroundJob = Hangfire.BackgroundJob;

namespace PharmacyManagement.Services.Implements
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitConversionRepository _unitConversionRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IReceiptRepository _receiptRepository;
        private readonly InvoiceBusinessValidator _businessValidator;
        private readonly InvoiceItemBusinessValidator _itemValidator;
        private readonly BatchSelectionStrategyFactory _strategyFactory;
        private readonly INotificationService _notificationService;
        public InvoiceService(
            IInvoiceRepository repository,
            IUserRepository userRepository,
            IUnitConversionRepository unitConversionRepository,
            IUnitRepository unitRepository,
            ICustomerRepository customerRepository,
            IReceiptRepository receiptRepository,
            InvoiceBusinessValidator businessValidator,
            InvoiceItemBusinessValidator itemValidator,
            BatchSelectionStrategyFactory strategyFactory,
            INotificationService notificationService
            )
        {
            _repository = repository;
            _userRepository = userRepository;
            _unitConversionRepository = unitConversionRepository;
            _unitRepository = unitRepository;
            _customerRepository = customerRepository;
            _receiptRepository = receiptRepository;
            _businessValidator = businessValidator;
            _itemValidator = itemValidator;
            _strategyFactory = strategyFactory;
            _notificationService = notificationService;
        }

        public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, long userId, long branchId)
        {
            // Validate customer tồn tại từ request
            Customer customer = await _businessValidator.ValidateCustomerExistsAsync(request.CustomerID);

            // validate người nhập tồn tại trong chi nhánh
            await ValidateUserInBranchAsync(request.CreatedByUserID, branchId);

            // validate (số lượng tồn kho) trong chi tiết hóa đơn theo FEFO hoặc Manual,
            // validate unit conversion, và tính số lượng quy đổi (base-unit) cho từng item
            var preparedItems = await PrepareItemsAsync(request.InvoiceItems, request.Mode, branchId);

            // tạo entity từ request và lưu vào database (Invoice)
            var invoice = request.ToEntity(userId, branchId);
            await _repository.AddAsync(invoice);

            await _repository.BeginTransactionAsync();
            try
            {
                // sử dụng strategy pattern để chọn batch theo FEFO hoặc Manual,
                // tạo InvoiceItem và trừ tồn kho (có điều kiện) trong cùng transaction
                invoice.TotalAmount = await ApplyAllocationsAsync(invoice, preparedItems, request.Mode, branchId);

                // lưu Invoice trước để có được InvoiceID thật
                // (cần nó để nối phiếu thu / sổ phụ ví vào hóa đơn vừa tạo)
                await _repository.SaveChangesAsync();

                // áp dụng thanh toán: ví (nếu khách đồng ý) + tiền mặt trả trước
                await ApplyWalletAndCashPaymentAsync(invoice, customer, request.PaidAmount, request.UseWalletAmount, branchId, userId);

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }

            // gửi thông báo mua hàng thành công
            var email = customer.Email;
            if (email != null && email.Contains("@"))
            {
                backgroundJob.Enqueue<INotificationService>(
                    notifier => notifier.SendInvoiceCreatedAsync(
                        customer.CustomerName, email, invoice.InvoiceID, invoice.CreatedAt, invoice.TotalAmount)
                );
            }

            var result = await _repository.GetByIdAsync(invoice.InvoiceID);
            return result!.ToResponse();
        }

        public async Task<InvoiceResponse> UpdateAsync(long id, UpdateInvoiceRequest request, long userId, long branchId)
        {
            // kiểm tra hóa đơn tồn tại trong database
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice == null)
                throw new BusinessException("Invoice not found.", "INV008", StatusCodes.Status404NotFound);

            // Validate customer tồn tại từ request
            await _businessValidator.ValidateCustomerExistsAsync(request.CustomerID);

            // validate người nhập tồn tại trong chi nhánh
            await ValidateUserInBranchAsync(request.CreatedByUserID, branchId);

            // validate và tính số lượng quy đổi cho từng chi tiết trong request
            var preparedItems = await PrepareItemsAsync(request.InvoiceItems, request.Mode, branchId);

            request.ApplyTo(invoice);

            await _repository.BeginTransactionAsync();
            try
            {
                // nếu hóa đơn đã có chi tiết hóa đơn trước đó,
                // cần hoàn trả số lượng thuốc trong các batch đã được phân bổ trước đó
                if (invoice.InvoiceItem is { Count: > 0 })
                {
                    foreach (var oldItem in invoice.InvoiceItem)
                    {
                        if (oldItem.Batch != null)
                            await _repository.IncrementStockAsync(oldItem.BatchID, oldItem.ConversionFactor);
                    }

                    _repository.RemoveInvoiceItems(invoice.InvoiceItem);
                    invoice.InvoiceItem.Clear();
                }

                invoice.TotalAmount = await ApplyAllocationsAsync(invoice, preparedItems, request.Mode, branchId);
                invoice.PaidAmount = request.PaidAmount;

                // cập nhật trạng thái thanh toán theo số tiền đã trả mới nhất
                invoice.PaymentStatus =
                    invoice.PaidAmount >= invoice.TotalAmount
                        ? PaymentStatus.Paid
                        : PaymentStatus.Debt;

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }

            var result = await _repository.GetByIdAsync(id);
            return result!.ToResponse();
        }

        public async Task DeleteAsync(long id, long branchId)
        {
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice == null)
                throw new BusinessException("Invoice not found.", "INV008", StatusCodes.Status404NotFound);

            await _repository.BeginTransactionAsync();
            try
            {
                if (invoice.InvoiceItem is { Count: > 0 })
                {
                    foreach (var item in invoice.InvoiceItem)
                    {
                        if (item.Batch != null)
                            await _repository.IncrementStockAsync(item.BatchID, item.ConversionFactor);
                    }
                }

                _repository.Delete(invoice);
                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task ValidateUserInBranchAsync(long? createdByUserID, long branchId)
        {
            if (!createdByUserID.HasValue) return;

            var userBranch = await _userRepository.GetUserBranchAsync(createdByUserID.Value, branchId);
            if (userBranch == null)
                throw new BusinessException("User not found in this branch.", "INV006", StatusCodes.Status404NotFound);
        }

        // Áp dụng thanh toán cho hóa đơn vừa tạo (phải gọi sau khi đã biết TotalAmount)
        private async Task ApplyWalletAndCashPaymentAsync(
            Invoice invoice, Customer customer,
            decimal cashPaid, decimal? requestedWalletAmount,
            long branchId, long userId)
        {
            // 1) Thanh toán bằng ví (nếu khách đồng ý dùng số dư trong ví)
            var credit = Math.Min(
                Math.Min(requestedWalletAmount ?? 0, customer.WalletBalance),
                invoice.TotalAmount);

            if (credit > 0)
            {
                await _receiptRepository.UpdateCustomerWalletAsync(customer.CustomerID, -credit);

                await _receiptRepository.AddWalletHistoryAsync(new CustomerWalletHistory
                {
                    CustomerID = customer.CustomerID,
                    TransactionType = WalletTransactionType.OUT,
                    Amount = credit,
                    RefType = WalletRefType.INVOICE,
                    RefId = invoice.InvoiceID,
                    CreateDate = DateTime.Now
                });

                invoice.PaidAmount += credit;

                // Tạo phiếu thu loại WALLET + 1 dòng gạch nợ cho hóa đơn vừa tạo
                await _receiptRepository.AddAsync(CreatePaymentReceipt(invoice, credit, PaymentMethod.WALLET, branchId, userId));
            }

            // 2) Thanh toán bằng tiền mặt (phần còn lại)
            var remaining = invoice.TotalAmount - credit;
            if (cashPaid > remaining)
                throw new BusinessException(
                    "Cash paid cannot exceed the remaining amount.", "INV013", StatusCodes.Status400BadRequest);

            if (cashPaid > 0)
            {
                invoice.PaidAmount += cashPaid;

                // Tạo phiếu thu loại CASH + 1 dòng gạch nợ cho hóa đơn vừa tạo
                await _receiptRepository.AddAsync(CreatePaymentReceipt(invoice, cashPaid, PaymentMethod.CASH, branchId, userId));
            }

            // 3) Cập nhật trạng thái thanh toán của hóa đơn
            invoice.PaymentStatus =
                invoice.PaidAmount >= invoice.TotalAmount
                    ? PaymentStatus.Paid
                    : PaymentStatus.Debt;
        }

        // Tạo 1 phiếu thu (tự động) kèm 1 dòng gạch nợ cho hóa đơn
        // Dùng navigation (Invoice = invoice) để EF tự gán InvoiceID sau khi lưu
        private Receipt CreatePaymentReceipt(Invoice invoice, decimal amount, PaymentMethod method, long branchId, long userId)
        {
            var receipt = new Receipt
            {
                CustomerID = invoice.CustomerID,
                BranchID = branchId,
                UserID = userId,
                TotalAmount = amount,
                PaymentMethod = method,
                CreatedDate = DateTime.Now
            };

            receipt.ReceiptDetail = new List<ReceiptDetail>
            {
                new ReceiptDetail { Invoice = invoice, AmountApplied = amount }
            };

            return receipt;
        }


        private async Task<List<PreparedInvoiceItem>> PrepareItemsAsync(
            IEnumerable<IInvoiceItemRequest> items, BatchSelectionMode mode, long branchId)
        {
            var itemsList = items as IReadOnlyList<IInvoiceItemRequest> ?? items.ToList();

            var medicineIds = itemsList.Select(i => i.MedicineID).Distinct().ToList();
            var unitIds = itemsList.Select(i => i.UnitID).Distinct().ToList();

            var medicines = await _repository.GetMedicinesByIdsAsync(medicineIds);
            var units = await _unitRepository.GetByIdsAsync(unitIds);
            var conversions = await _unitConversionRepository.GetByMedicineIdsAsync(medicineIds);

            var medicineById = medicines.ToDictionary(m => m.MedicineID);
            var unitById = units.ToDictionary(u => u.UnitID);
            var conversionLookup = conversions
                .GroupBy(c => (c.MedicineID, c.UnitID))
                .ToDictionary(g => g.Key, g => g.First());

            Dictionary<long, Batch>? manualBatches = null;
            Dictionary<long, List<Batch>>? batchesByMedicine = null;

            if (mode == BatchSelectionMode.FEFO)
            {
                batchesByMedicine = await _repository.GetBatchesByMedicineIdsAsync(medicineIds, branchId);
            }
            else
            {
                var batchIds = itemsList
                    .Where(i => i.BatchID.HasValue)
                    .Select(i => i.BatchID!.Value)
                    .Distinct()
                    .ToList();
                manualBatches = await _repository.GetBatchesByIdsAsync(batchIds);
            }

            var result = new List<PreparedInvoiceItem>(itemsList.Count);

            foreach (var itemRequest in itemsList)
            {
                if (!medicineById.TryGetValue(itemRequest.MedicineID, out var medicine))
                    throw new BusinessException("Medicine not found.", "MED001", StatusCodes.Status404NotFound);

                unitById.TryGetValue(itemRequest.UnitID, out var unit);

                decimal conversionRate;
                decimal baseUnitQuantity;

                // nếu UnitID == BaseUnitID → ConversionRate=1, BaseUnitQuantity=Quantity
                // else UnitID != BaseUnitID → lookup UnitConversion(MedicineID, UnitID) → ConversionRate=Factor, BaseUnitQuantity=Quantity*Factor
                if (itemRequest.UnitID == medicine.BaseUnitID)
                {
                    conversionRate = 1;
                    baseUnitQuantity = itemRequest.Quantity;
                }
                else
                {
                    if (!conversionLookup.TryGetValue((itemRequest.MedicineID, itemRequest.UnitID), out var unitConversion))
                        throw new BusinessException(
                            $"No unit conversion found for medicine {itemRequest.MedicineID} and unit {itemRequest.UnitID}.",
                            "INV011",
                            StatusCodes.Status400BadRequest);

                    conversionRate = unitConversion.Factor;
                    baseUnitQuantity = conversionRate * itemRequest.Quantity;
                }

                Batch? manualBatch = null;
                IReadOnlyList<Batch>? availableBatches = null;

                if (mode == BatchSelectionMode.FEFO)
                {
                    var available = batchesByMedicine!.TryGetValue(itemRequest.MedicineID, out var batches)
                        ? batches.Sum(b => b.QuantityInStock)
                        : 0;
                    _itemValidator.ValidateFefoItemAsync(itemRequest, baseUnitQuantity, available);
                    availableBatches = batches;
                }
                else
                {
                    var manualBatchEntity = itemRequest.BatchID.HasValue
                        && manualBatches!.TryGetValue(itemRequest.BatchID.Value, out var mb)
                            ? mb
                            : null;
                    manualBatch = _itemValidator.ValidateManualItemAsync(itemRequest, baseUnitQuantity, manualBatchEntity!);
                }

                result.Add(new PreparedInvoiceItem(
                    itemRequest, baseUnitQuantity, conversionRate, unit?.UnitName ?? string.Empty, manualBatch, availableBatches));
            }

            return result;
        }

        private async Task<decimal> ApplyAllocationsAsync(
            Invoice invoice,
            IEnumerable<PreparedInvoiceItem> preparedItems,
            BatchSelectionMode mode,
            long branchId)
        {

            invoice.InvoiceItem ??= new List<InvoiceItem>();

            var strategy = _strategyFactory.GetStrategy(mode);

            foreach (var item in preparedItems)
            {
                // lấy medicine và unit từ preparedItems
                // và lấy batch từ strategy để tạo InvoiceItem
                var allocations = await strategy.ResolveBatchesAsync(item, branchId);

                foreach (var alloc in allocations)
                {
                    var batch = alloc.Batch
                        ?? throw new BusinessException("Batch not found.", "INV002", StatusCodes.Status404NotFound);

                    invoice.InvoiceItem.Add(item.Request.ToEntity(
                        invoice.InvoiceID, batch, item.BaseUnitQuantity, item.ConversionRate, item.UnitName));

                    // trừ tồn kho có điều kiện để tránh bán vượt stock khi có request đồng thời
                    if (!await _repository.TryDecrementStockAsync(alloc.BatchID, alloc.Quantity))
                        throw new BusinessException(
                            $"Insufficient stock for batch ID {alloc.BatchID}.",
                            "INV004",
                            StatusCodes.Status400BadRequest);
                }
            }

            // tính tổng tiền của hóa đơn dựa trên các InvoiceItem đã tạo
            return invoice.InvoiceItem.Sum(ii => ii.Quantity * ii.UnitPrice);
        }

        public async Task<InvoiceDetailResponse?> GetByIdAsync(long id, long branchId)
        {
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice == null || invoice.BranchID != branchId)
                return null;

            return invoice.ToDetailResponse();
        }

        public async Task<InvoiceListResponse> GetAllAsync(InvoiceFilterDto filter, long branchId)
        {
            var query = _repository.GetQuery()
                .FilterByBranch(branchId)
                .FilterByKeyword(filter.Keyword)
                .FilterByDate(filter.FromDate, filter.ToDate)
                .FilterByCustomer(filter.CustomerID)
                .FilterByUser(filter.UserID)
                .FilterByAmount(filter.MinTotalAmount, filter.MaxTotalAmount)
                .ApplySort(filter.SortBy, filter.IsDescending);

            var paged = await query.ToPagedResultAsync(filter);

            return new InvoiceListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Invoices = paged.Items.Select(i => new InvoiceResponse
                {
                    InvoiceID = i.InvoiceID,
                    CustomerName = i.Customer != null ? i.Customer.CustomerName : string.Empty,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    CreatedAt = i.CreatedAt,
                    UserName = i.User != null ? i.User.FullName : string.Empty
                }).ToList()
            };
        }

        public async Task<List<BatchByMedicineResponse>> GetBatchesByMedicineAsync(long medicineID, long branchID)
        {
            var batches = await _repository.GetBatchesByMedicineAsync(medicineID, branchID);
            return batches.Select(b => b.ToBatchResponse()).ToList();
        }

        public async Task<FefoResultResponse> GetFefoBatchesAsync(long medicineID, decimal quantity, long branchID)
        {
            var batches = await _repository.GetBatchesByMedicineAsync(medicineID, branchID);

            var allocated = new List<AllocatedBatchDto>();
            decimal remaining = quantity;

            foreach (var batch in batches)
            {
                if (remaining <= 0) break;
                decimal alloc = Math.Min(batch.QuantityInStock, remaining);
                allocated.Add(batch.ToAllocatedDto(alloc));
                remaining -= alloc;
            }

            return new FefoResultResponse
            {
                MedicineID = medicineID,
                MedicineName = batches.FirstOrDefault()?.GoodsReceiptItem?.Medicine?.MedicineName ?? string.Empty,
                TotalQuantityRequested = quantity,
                Fulfilled = remaining <= 0,
                AllocatedBatches = allocated
            };
        }

        public async Task<List<UserByBranchResponse>> GetUsersByBranchAsync(long branchID)
        {
            var users = await _userRepository.GetAllUsersByBranchAsync(branchID, 0, 1000);
            return users.Select(u => new UserByBranchResponse
            {
                UserID = u.UserID,
                UserName = u.UserName,
                FullName = u.FullName
            }).ToList();
        }
    }
}
