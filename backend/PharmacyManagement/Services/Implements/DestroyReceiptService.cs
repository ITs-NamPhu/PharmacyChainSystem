using PharmacyManagement.DTOs.DestroyReceipt;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class DestroyReceiptService : IDestroyReceiptService
    {
        private readonly IDestroyReceiptRepository _repository;
        private readonly DestroyReceiptBusinessValidator _businessValidator;

        public DestroyReceiptService(
            IDestroyReceiptRepository repository,
            DestroyReceiptBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<DestroyReceiptDetailResponse> CreateAsync(CreateDestroyReceiptRequest request, long userId)
        {
            if (request.Items.Count == 0)
                throw new BusinessException("Destroy receipt must contain at least one item.", "DR010", StatusCodes.Status400BadRequest);

            await _businessValidator.ValidateWarehouseExistsAsync(request.WarehouseID);
            await _businessValidator.ValidateStockTakeCanDestroyAsync(request.StockTakeID, request.WarehouseID);

            var batchIds = request.Items.Select(i => i.BatchID).ToList();
            var batches = await _businessValidator.ValidateBatchesInWarehouseAsync(batchIds, request.WarehouseID);


            var stockTakeItemIds = request.Items
                .Where(i => i.StockTakeItemID.HasValue)
                .Select(i => i.StockTakeItemID!.Value);

            // lấy ds stock take item từ repository
            var stockTakeItems = await _repository.GetStockTakeItemsByIdsAsync(stockTakeItemIds);

            _businessValidator.ValidateStockTakeItemLinksAsync(
                request.Items.Select(i => (i.StockTakeItemID, i.BatchID)),
                request.StockTakeID,
                stockTakeItems);

            foreach (var item in request.Items)
            {
                _businessValidator.ValidateDestroyQuantity(batches[item.BatchID], item.Quantity);
            }

            var destroyReceipt = request.ToEntity(userId);

            destroyReceipt.DestroyReceiptItem = request.Items
                .Select(item => item.ToEntity(0, batches[item.BatchID].GoodsReceiptItem?.UnitCost ?? 0))
                .ToList();

            await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddAsync(destroyReceipt);

                foreach (var item in destroyReceipt.DestroyReceiptItem!)
                {
                    batches[item.BatchID].QuantityInStock -= item.Quantity;
                }

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }

            return (await GetByIdAsync(destroyReceipt.DestroyReceiptID))!;
        }

        public async Task ApproveAsync(long id, long userId)
        {
            var destroyReceipt = await _repository.GetByIdAsync(id);
            if (destroyReceipt == null)
                throw new BusinessException("Destroy receipt not found.", "DR009", StatusCodes.Status404NotFound);

            destroyReceipt.ApprovedBy = userId;
            destroyReceipt.ApprovedAt = DateTime.Now;
            await _repository.SaveChangesAsync();
        }

        public async Task<DestroyReceiptDetailResponse?> GetByIdAsync(long id)
        {
            var destroyReceipt = await _repository.GetByIdAsync(id);
            return destroyReceipt?.ToDetailResponse();
        }

        public async Task<DestroyReceiptListResponse> GetAllAsync(int page, int count, long warehouseId)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                (skip, take) => _repository.GetAllAsync(skip, take, warehouseId),
                () => _repository.CountAsync(warehouseId),
                page, count);

            return new DestroyReceiptListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                DestroyReceipts = paged.Items.ToResponseList()
            };
        }
    }
}
