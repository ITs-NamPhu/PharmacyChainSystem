using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class StockAdjustmentBusinessValidator
    {
        private readonly IStockAdjustmentRepository _repository;

        public StockAdjustmentBusinessValidator(IStockAdjustmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<StockTake> ValidateStockTakeCanAdjustAsync(long stockTakeId)
        {
            var stockTake = await _repository.GetStockTakeByIdAsync(stockTakeId);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "SA001", StatusCodes.Status404NotFound);

            if (stockTake.Status != StockTakeStatus.Completed)
                throw new BusinessException(
                    "Stock take must be completed before creating an adjustment.",
                    "SA002",
                    StatusCodes.Status400BadRequest);

            if (!stockTake.IsAdjust)
                throw new BusinessException(
                    "Stock take has no items flagged for adjustment.",
                    "SA004",
                    StatusCodes.Status400BadRequest);

            return stockTake;
        }

        public async Task EnsureNoAdjustmentExistsAsync(long stockTakeId)
        {
            if (await _repository.HasAdjustmentAsync(stockTakeId))
                throw new BusinessException(
                    "Stock take has already been adjusted.",
                    "SA005",
                    StatusCodes.Status400BadRequest);
        }

        public async Task<Dictionary<long, Batch>> ValidateBatchesInWarehouseAsync(
            IEnumerable<long> batchIds,
            long warehouseId)
        {
            var batches = await _repository.GetBatchesByIdsAsync(batchIds);

            foreach (var batchId in batchIds)
            {
                if (!batches.TryGetValue(batchId, out var batch))
                    throw new BusinessException($"Batch ID {batchId} not found.", "SA006", StatusCodes.Status404NotFound);

                if (batch.WarehouseID != warehouseId)
                    throw new BusinessException(
                        $"Batch ID {batchId} does not belong to warehouse ID {warehouseId}.",
                        "SA007",
                        StatusCodes.Status400BadRequest);
            }

            return batches;
        }

        public void ValidateStockTakeItemLink(
            long? stockTakeItemId,
            ICollection<StockTakeItem> stockTakeItems,
            long stockTakeId,
            long batchId)
        {
            if (!stockTakeItemId.HasValue)
                return;

            var item = stockTakeItems.FirstOrDefault(i => i.StockTakeItemID == stockTakeItemId.Value);
            if (item == null)
                throw new BusinessException($"Stock take item ID {stockTakeItemId} not found.", "SA008", StatusCodes.Status404NotFound);

            if (item.StockTakeID != stockTakeId)
                throw new BusinessException(
                    "Stock take item does not belong to the given stock take.",
                    "SA009",
                    StatusCodes.Status400BadRequest);

            if (item.BatchID != batchId)
                throw new BusinessException(
                    "Stock take item does not belong to the given batch.",
                    "SA010",
                    StatusCodes.Status400BadRequest);

            if (!item.IsAdjust)
                throw new BusinessException(
                    $"Stock take item ID {stockTakeItemId} is not flagged for adjustment.",
                    "SA015",
                    StatusCodes.Status400BadRequest);
        }

        public void ValidateStockNotNegative(Batch batch, decimal adjustQuantity)
        {
            if (batch.QuantityInStock + adjustQuantity < 0)
                throw new BusinessException(
                    $"Insufficient stock for batch ID {batch.BatchID}. " +
                    $"Available: {batch.QuantityInStock}, adjustment: {adjustQuantity}.",
                    "SA011",
                    StatusCodes.Status400BadRequest);
        }
    }
}
