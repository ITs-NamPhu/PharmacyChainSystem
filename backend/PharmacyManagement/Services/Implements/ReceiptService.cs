using Microsoft.AspNetCore.Http;
using PharmacyManagement.DTOs.Receipt;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using backgroundJob = Hangfire.BackgroundJob;

namespace PharmacyManagement.Services.Implements
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;
        private readonly INotificationService _notificationService;

        public ReceiptService(IReceiptRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request, long userId, long branchId)
        {
            // BƯỚC 0: Kiểm tra dữ liệu đầu vào
            if (request.TotalAmount <= 0)
                throw new BusinessException("TotalAmount must be greater than zero.", "RCP001", StatusCodes.Status400BadRequest);

            var customer = await _repository.GetCustomerAsync(request.CustomerID);
            if (customer == null)
                throw new BusinessException("Customer not found.", "RCP002", StatusCodes.Status404NotFound);

            // BƯỚC 1: Mở Transaction để đảm bảo dữ liệu không bị lỗi nửa chừng
            await _repository.BeginTransactionAsync();
            try
            {
                // BƯỚC 2: Lấy các hóa đơn còn nợ của khách, hóa đơn cũ nhất đứng trước (FIFO)
                var unpaidInvoices = await _repository.GetUnpaidInvoicesFifoAsync(request.CustomerID);

                // BƯỚC 3: Lưu phiếu thu = toàn bộ số tiền khách thực đưa vào két
                var receipt = request.ToEntity(userId, branchId);
                await _repository.AddAsync(receipt);
                await _repository.SaveChangesAsync(); // để lấy được ReceiptID

                // BƯỚC 4: Dùng thuật toán FIFO để sinh các dòng gạch nợ
                var details = ApplyFifo(request.TotalAmount, unpaidInvoices, receipt.ReceiptID);
                await _repository.AddDetailsRangeAsync(details);

                // BƯỚC 5: Cập nhật PaidAmount + PaymentStatus của từng hóa đơn
                foreach (var detail in details)
                {
                    var invoice = unpaidInvoices.Find(i => i.InvoiceID == detail.InvoiceID)!;
                    invoice.PaidAmount += detail.AmountApplied;
                    invoice.PaymentStatus =
                        invoice.PaidAmount >= invoice.TotalAmount
                            ? PaymentStatus.Paid
                            : PaymentStatus.Debt;
                }

                // BƯỚC 6: Nếu khách đưa dư tiền thì nạp phần dư vào ví
                var appliedTotal = details.Sum(d => d.AmountApplied);
                var excess = request.TotalAmount - appliedTotal;
                if (excess > 0)
                {
                    // nạp phần dư vào ví khách hàng (cập nhật atomic trong DB)
                    await _repository.UpdateCustomerWalletAsync(customer.CustomerID, excess);
                    await _repository.AddWalletHistoryAsync(new CustomerWalletHistory
                    {
                        CustomerID = customer.CustomerID,
                        TransactionType = WalletTransactionType.IN,
                        Amount = excess,
                        RefType = WalletRefType.RECEIPT,
                        RefId = receipt.ReceiptID,
                        CreateDate = DateTime.Now
                    });
                }

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();

                // BƯỚC 7: Trả kết quả kèm chi tiết gạch nợ
                var saved = await _repository.GetByIdAsync(receipt.ReceiptID);
                var result = saved!.ToResponse();
                result.RemainingDebt = unpaidInvoices.Sum(i => i.TotalAmount - i.PaidAmount);

                // Gửi thông báo phiếu thu đã được tạo thành công
                if (customer.Email is not null && customer.Email.Contains("@"))
                {
                    backgroundJob.Enqueue<INotificationService>(
                        notifier => notifier.SendReceiptCreatedAsync(
                            customer.CustomerName, customer.Email, receipt.ReceiptID, receipt.CreatedDate, receipt.TotalAmount)
                    );
                }

                return result;
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }
        }

        // Thuật toán FIFO: hóa đơn phát sinh nợ trước được ưu tiên trừ tiền trước
        private List<ReceiptDetail> ApplyFifo(decimal receivedAmount, List<Invoice> unpaidInvoices, long receiptId)
        {
            var details = new List<ReceiptDetail>();
            var amountLeft = receivedAmount;

            foreach (var invoice in unpaidInvoices)
            {
                if (amountLeft <= 0) break; // hết tiền rồi thì dừng

                // Lấy số tiền nhỏ hơn giữa (tiền còn lại của khách) và (tiền nợ của hóa đơn)
                var remainingDebt = invoice.TotalAmount - invoice.PaidAmount;
                var amountToApply = Math.Min(amountLeft, remainingDebt);

                details.Add(new ReceiptDetail
                {
                    ReceiptID = receiptId,
                    InvoiceID = invoice.InvoiceID,
                    AmountApplied = amountToApply
                });

                amountLeft -= amountToApply; // trừ đi số tiền vừa gạch nợ
            }

            return details;
        }

        public async Task<ReceiptResponse> UpdateAsync(long id, UpdateReceiptRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Receipt not found.", "RCP003", StatusCodes.Status404NotFound);

            // Không cho phép sửa phiếu thu đã gạch nợ để giữ FIFO và đối soát được chính xác
            if (await _repository.HasDetailsAsync(id))
                throw new BusinessException(
                    "Cannot update a receipt that already applied to invoices.", "RCP004", StatusCodes.Status400BadRequest);

            if (request.TotalAmount <= 0)
                throw new BusinessException("TotalAmount must be greater than zero.", "RCP001", StatusCodes.Status400BadRequest);

            if (!await _repository.IsCustomerExistsAsync(request.CustomerID))
                throw new BusinessException("Customer not found.", "RCP002", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            var saved = await _repository.GetByIdAsync(id);
            return saved!.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Receipt not found.", "RCP003", StatusCodes.Status404NotFound);

            // Không cho phép xóa phiếu thu đã gạch nợ để giữ FIFO và đối soát được chính xác
            if (await _repository.HasDetailsAsync(id))
                throw new BusinessException(
                    "Cannot delete a receipt that already applied to invoices.", "RCP005", StatusCodes.Status400BadRequest);

            // HandleSoftDelete() trong DbContext sẽ tự chuyển DELETE thành UPDATE IsDeleted = true
            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<ReceiptResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var result = entity.ToResponse();

            // Tính tổng nợ còn lại của khách tại thời điểm xem phiếu thu
            var unpaidInvoices = await _repository.GetUnpaidInvoicesFifoAsync(entity.CustomerID);
            result.RemainingDebt = unpaidInvoices.Sum(i => i.TotalAmount - i.PaidAmount);

            return result;
        }

        public async Task<ReceiptListResponse> GetAllAsync(long? customerId, int page, int count)
        {
            var skip = PaginationHelper.GetSkip(page, count);

            var items = await _repository.GetAllAsync(skip, count, customerId);
            var numRecords = await _repository.CountAsync(customerId);

            return new ReceiptListResponse
            {
                NumRecords = numRecords,
                TotalPage = (int)PaginationHelper.GetTotalPage(numRecords, count),
                Items = items.ToResponseList()
            };
        }
    }
}