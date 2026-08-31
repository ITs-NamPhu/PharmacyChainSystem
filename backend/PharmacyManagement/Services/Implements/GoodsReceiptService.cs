using PharmacyManagement.DTOs.GoodsReceipt;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Extensions;
using PharmacyManagement.Handlers;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class GoodsReceiptService : IGoodsReceiptService
    {
        private readonly IGoodsReceiptRepository _repository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUnitConversionRepository _unitConversionRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly GoodsReceiptBusinessValidator _businessValidator;
        private readonly GoodsReceiptItemUpdateHandler _itemUpdateHandler;

        public GoodsReceiptService(
            IGoodsReceiptRepository repository,
            IMedicineRepository medicineRepository,
            IUnitRepository unitRepository,
            IUnitConversionRepository unitConversionRepository,
            IWarehouseRepository warehouseRepository,
            GoodsReceiptBusinessValidator businessValidator,
            GoodsReceiptItemUpdateHandler itemUpdateHandler)
        {
            _repository = repository;
            _medicineRepository = medicineRepository;
            _unitRepository = unitRepository;
            _unitConversionRepository = unitConversionRepository;
            _warehouseRepository = warehouseRepository;
            _businessValidator = businessValidator;
            _itemUpdateHandler = itemUpdateHandler;
        }

        private async Task<Medicine> GetOrCacheMedicineAsync(long medicineId, Dictionary<long, Medicine> cache)
        {
            if (cache.TryGetValue(medicineId, out var cached))
                return cached;

            var medicine = await _medicineRepository.GetByIdAsync(medicineId);
            if (medicine == null)
                throw new BusinessException("Medicine not found.", "GR011", StatusCodes.Status404NotFound);

            cache[medicineId] = medicine;
            return medicine;
        }

        private async Task<Unit> GetOrCacheUnitAsync(long unitId, Dictionary<long, Unit> cache)
        {
            if (cache.TryGetValue(unitId, out var cached))
                return cached;

            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null)
                throw new BusinessException("Unit not found.", "GR012", StatusCodes.Status404NotFound);

            cache[unitId] = unit;
            return unit;
        }

        public async Task<GoodsReceiptResponse> CreateAsync(CreateGoodsReceiptRequest request, long userId, long branchId)
        {
            // validate nhà cung cấp
            await _businessValidator.ValidateSupplierExistsAsync(request.SupplierID);

            // Khởi tạo dic cho cache thuốc và đơn vị tính để tránh truy vấn nhiều lần
            var medicineCache = new Dictionary<long, Medicine>();
            var unitCache = new Dictionary<long, Unit>();


            foreach (var item in request.Items)
            {
                await GetOrCacheMedicineAsync(item.MedicineID, medicineCache);
                await GetOrCacheUnitAsync(item.UnitID, unitCache);
            }

            var receiptNumber = await _repository.GetNextReceiptNumberAsync();
            var goodsReceipt = request.ToEntity(userId, branchId);
            goodsReceipt.ReceiptNumber = receiptNumber;

            await _repository.AddAsync(goodsReceipt);
            await _repository.SaveChangesAsync();

            decimal totalAmount = 0;
            var receiptItems = new List<(CreateGoodsReceiptItemRequest Request, GoodsReceiptItem Entity)>();
            var conversionCache = new Dictionary<(long MedicineId, long UnitId), UnitConversion?>();

            foreach (var itemRequest in request.Items)
            {
                var medicine = medicineCache[itemRequest.MedicineID];
                var unit = unitCache[itemRequest.UnitID];

                decimal conversionFactor;
                string unitName = unit.UnitName;

                if (itemRequest.UnitID == medicine.BaseUnitID)
                {
                    conversionFactor = itemRequest.Quantity;
                }
                else
                {
                    var key = (itemRequest.MedicineID, itemRequest.UnitID);
                    if (!conversionCache.TryGetValue(key, out var unitConversion))
                    {
                        unitConversion = await _unitConversionRepository
                            .GetByMedicineAndUnitAsync(itemRequest.MedicineID, itemRequest.UnitID);
                        conversionCache[key] = unitConversion;
                    }
                    conversionFactor = (unitConversion?.Factor ?? 1) * itemRequest.Quantity;
                }

                var goodsReceiptItem = itemRequest.ToEntity(goodsReceipt.GoodsReceiptID, conversionFactor, unitName);
                goodsReceipt.GoodsReceiptItem ??= new List<GoodsReceiptItem>();
                goodsReceipt.GoodsReceiptItem.Add(goodsReceiptItem);
                receiptItems.Add((itemRequest, goodsReceiptItem));

                totalAmount += itemRequest.Quantity * itemRequest.UnitCost;
            }

            goodsReceipt.TotalAmount = totalAmount;

            var warehouses = await _warehouseRepository.GetByBranchAsync(branchId, 0, 1);
            var warehouseId = warehouses.FirstOrDefault()?.WarehouseID ?? 0;

            // duyệt qua từng item trong receiptItems để tạo batch
            foreach (var (itemRequest, goodsReceiptItem) in receiptItems)
            {
                var batch = new Batch
                {
                    GoodsReceiptItemID = goodsReceiptItem.GoodsReceiptItemID,
                    WarehouseID = warehouseId,
                    QuantityReceived = goodsReceiptItem.ConversionFactor,
                    QuantityInStock = goodsReceiptItem.ConversionFactor,
                    ManufactureDate = itemRequest.ManufactureDate,
                    ExpiryDate = itemRequest.ExpiryDate,
                    Note = string.Empty
                };
                goodsReceiptItem.Batch ??= new List<Batch>();
                goodsReceiptItem.Batch.Add(batch);
            }

            await _repository.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(goodsReceipt.GoodsReceiptID);
            return result!.ToResponse();
        }

        public async Task<GoodsReceiptResponse> UpdateAsync(long id, UpdateGoodsReceiptRequest request, long userId, long branchId)
        {
            // 1. Validate chung
            // Phiếu nhập tồn tại và thuộc chi nhánh của người sửa, nhà cung cấp
            // Validate các thuốc, đơn vị tính trong items
            var goodsReceipt = await _repository.GetByIdAsync(id);
            if (goodsReceipt == null)
                throw new BusinessException("Goods receipt not found.", "GR002", StatusCodes.Status404NotFound);

            if (goodsReceipt.BranchID != branchId)
                throw new BusinessException("Goods receipt not found in this branch.", "GR003", StatusCodes.Status403Forbidden);

            await _businessValidator.ValidateSupplierExistsAsync(request.SupplierID);

            // không cho phép gửi trùng GoodsReceiptItemID trong request
            var duplicateItemIds = request.Items
                                        .Where(i => i.GoodsReceiptItemID.HasValue)
                                        .GroupBy(i => i.GoodsReceiptItemID!.Value)
                                        .Where(g => g.Count() > 1)
                                        .Select(g => g.Key)
                                        .ToList();

            if (duplicateItemIds.Any())
                throw new BusinessException("Duplicate GoodsReceiptItemID in request.", "GR013", StatusCodes.Status400BadRequest);

            // validate + cache thuốc, đơn vị tính trong items
            var medicineCache = new Dictionary<long, Medicine>();
            var unitCache = new Dictionary<long, Unit>();
            foreach (var item in request.Items)
            {
                await GetOrCacheMedicineAsync(item.MedicineID, medicineCache);
                await GetOrCacheUnitAsync(item.UnitID, unitCache);
            }

            // chuyển các thông tin từ request sang entity
            request.ApplyTo(goodsReceipt);


            // 2. Phân loại items
            // Lấy danh sách items của phiếu nhập từ database
            var existingItems = goodsReceipt.GoodsReceiptItem?.ToList() ?? new List<GoodsReceiptItem>();

            // Lấy danh sách items trong REQUEST có GoodsReceiptItemID (các items cần update)
            var requestItemIds = request.Items
                                        .Where(i => i.GoodsReceiptItemID.HasValue)
                                        .Select(i => i.GoodsReceiptItemID!.Value)
                                        .ToHashSet();


            // 3. Validate xóa items
            // case: nếu item có trong database nhưng không có trong request (người dùng muốn xóa item đó)
            // Xử lý xóa item logic:
            //                      nếu item đã được xuất bán => không cho phép xóa,
            //                      nếu chưa xuất bán (unsold) => cho phép xóa
            foreach (var existingItem in existingItems)
            {
                if (!requestItemIds.Contains(existingItem.GoodsReceiptItemID))
                {
                    var status = existingItem.Batch?.FirstOrDefault() is { } b
                        ? BatchExportStatus.From(b)
                        : BatchExportStatus.NoBatch();

                    _itemUpdateHandler.ValidateItemDeletion(existingItem, status);
                }
            }

            // 4. Xóa items không còn trong request
            var itemsToRemove = existingItems
                .Where(i => !requestItemIds.Contains(i.GoodsReceiptItemID))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                _repository.RemoveGoodsReceiptItems(new[] { item });
            }



            // 5. Validate + Apply updates/creates
            decimal totalAmount = 0;
            var newItems = new List<(UpdateGoodsReceiptItemRequest Request, GoodsReceiptItem Entity)>();
            var conversionCache = new Dictionary<(long MedicineId, long UnitId), UnitConversion?>();

            foreach (var itemRequest in request.Items)
            {
                // lấy medicine và validate medicine exists
                var medicine = medicineCache[itemRequest.MedicineID];

                // lấy đơn vị tính và validate đơn vị tính exists
                var unit = unitCache[itemRequest.UnitID];

                decimal conversionFactor;
                string unitName = unit.UnitName;

                // nếu đơn vị tính trong request là đơn vị cơ sở của thuốc => conversionFactor = quantity
                // nếu không => lấy conversionFactor từ bảng UnitConversion * quantity(request)
                if (itemRequest.UnitID == medicine.BaseUnitID)
                {
                    conversionFactor = itemRequest.Quantity;
                }
                else
                {
                    var key = (itemRequest.MedicineID, itemRequest.UnitID);
                    if (!conversionCache.TryGetValue(key, out var unitConversion))
                    {
                        unitConversion = await _unitConversionRepository
                            .GetByMedicineAndUnitAsync(itemRequest.MedicineID, itemRequest.UnitID);
                        conversionCache[key] = unitConversion;
                    }
                    conversionFactor = (unitConversion?.Factor ?? 1) * itemRequest.Quantity;
                }

                // nếu itemRequest có GoodsReceiptItemID
                //  => validate (số lượng sửa > số lượng đã bán và medicine trong batch) và update item
                // nếu không => create item
                if (itemRequest.GoodsReceiptItemID.HasValue)
                {
                    var existingItem = existingItems
                        .FirstOrDefault(i => i.GoodsReceiptItemID == itemRequest.GoodsReceiptItemID.Value);

                    if (existingItem != null)
                    {
                        var status = existingItem.Batch?.FirstOrDefault() is { } b
                            ? BatchExportStatus.From(b)
                            : BatchExportStatus.NoBatch();

                        _itemUpdateHandler.ValidateItemUpdate(itemRequest, existingItem, status, conversionFactor);
                        _itemUpdateHandler.ApplyUpdate(itemRequest, existingItem, status, conversionFactor, unitName);
                    }
                }
                else
                {
                    var newItem = itemRequest.ToEntity(goodsReceipt.GoodsReceiptID, conversionFactor, unitName);
                    newItem.Medicine = medicine;
                    goodsReceipt.GoodsReceiptItem ??= new List<GoodsReceiptItem>();
                    goodsReceipt.GoodsReceiptItem.Add(newItem);
                    newItems.Add((itemRequest, newItem));
                }

                totalAmount += itemRequest.Quantity * itemRequest.UnitCost;
            }

            goodsReceipt.TotalAmount = totalAmount;

            // 6. Tạo batch cho items mới
            if (newItems.Any())
            {
                var warehouses = await _warehouseRepository.GetByBranchAsync(branchId, 0, 1);
                var warehouseId = warehouses.FirstOrDefault()?.WarehouseID ?? 0;

                foreach (var (itemRequest, newItem) in newItems)
                {
                    var batch = new Batch
                    {
                        GoodsReceiptItemID = newItem.GoodsReceiptItemID,
                        WarehouseID = warehouseId,
                        QuantityReceived = newItem.ConversionFactor,
                        QuantityInStock = newItem.ConversionFactor,
                        ManufactureDate = itemRequest.ManufactureDate,
                        ExpiryDate = itemRequest.ExpiryDate,
                        Note = string.Empty
                    };
                    newItem.Batch ??= new List<Batch>();
                    newItem.Batch.Add(batch);
                }
            }

            await _repository.SaveChangesAsync();

            return goodsReceipt.ToResponse();
        }

        public async Task DeleteAsync(long id, long branchId)
        {
            var goodsReceipt = await _repository.GetByIdAsync(id);
            if (goodsReceipt == null)
                throw new BusinessException("Goods receipt not found.", "GR002", StatusCodes.Status404NotFound);

            if (goodsReceipt.BranchID != branchId)
                throw new BusinessException("Goods receipt not found in this branch.", "GR003", StatusCodes.Status403Forbidden);

            // chỉ cho phép xóa khi batch của phiếu chưa được tham chiếu
            // (hóa đơn, trả NCC, trả hàng, hủy, chỉnh tồn, kiểm kê)
            var batchIds = goodsReceipt.GoodsReceiptItem?
                .SelectMany(i => i.Batch ?? new List<Batch>())
                .Select(b => b.BatchID)
                .ToList() ?? new List<long>();

            if (await _repository.HasAnyReferenceForBatchesAsync(batchIds))
            {
                throw new BusinessException(
                    "Cannot delete goods receipt because its batches have related records.",
                    "GR004",
                    StatusCodes.Status400BadRequest);
            }

            // soft delete phiếu + xóa cứng batch, giữ lại toàn bộ GoodsReceiptItem
            await _repository.BeginTransactionAsync();
            try
            {
                var batches = goodsReceipt.GoodsReceiptItem?
                    .SelectMany(i => i.Batch ?? new List<Batch>())
                    .ToList() ?? new List<Batch>();
                if (batches.Count > 0)
                    _repository.RemoveBatches(batches);

                // HandleSoftDelete() trong DbContext sẽ tự chuyển DELETE thành UPDATE IsDeleted = true
                _repository.Delete(goodsReceipt);

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<GoodsReceiptDetailResponse?> GetByIdAsync(long id, long branchId)
        {
            var goodsReceipt = await _repository.GetByIdAsync(id);
            if (goodsReceipt == null || goodsReceipt.BranchID != branchId)
                return null;

            return goodsReceipt.ToDetailResponse();
        }

        public async Task<GoodsReceiptListResponse> GetAllAsync(GoodsReceiptFilterDto filter, long branchId)
        {
            var query = _repository.GetQuery()
                .FilterByBranch(branchId)
                .FilterByKeyword(filter.Keyword)
                .FilterByDate(filter.FromDate, filter.ToDate)
                .FilterBySupplier(filter.SupplierID)
                .FilterByAmount(filter.MinTotalAmount, filter.MaxTotalAmount)
                .ApplySort(filter.SortBy, filter.IsDescending);

            var projectedQuery = query.Select(gr => new GoodsReceiptResponse
            {
                GoodsReceiptID = gr.GoodsReceiptID,
                ReceiptNumber = gr.ReceiptNumber,
                SupplierName = gr.Supplier != null ? gr.Supplier.SupplierName : string.Empty,
                UserName = gr.User != null ? gr.User.FullName : string.Empty,
                ReceiptDate = gr.ReceiptDate,
                TotalAmount = gr.TotalAmount,
                PaidAmount = gr.PaidAmount,
                Note = gr.Note
            });

            var paged = await projectedQuery.ToPagedResultAsync(filter);

            return new GoodsReceiptListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                GoodsReceipts = paged.Items
            };
        }
    }
}
