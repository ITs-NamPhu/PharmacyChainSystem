using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class DestroyReceiptBusinessValidator
    {
        private readonly IDestroyReceiptRepository _repository;

        public DestroyReceiptBusinessValidator(IDestroyReceiptRepository repository)
        {
            _repository = repository;
        }

        public async Task<WareHouse> ValidateWarehouseExistsAsync(long warehouseId)
        {
            var warehouse = await _repository.GetWarehouseByIdAsync(warehouseId);
            if (warehouse == null)
                throw new BusinessException("Warehouse not found.", "DR001", StatusCodes.Status404NotFound);

            return warehouse;
        }

        public async Task<Dictionary<long, Batch>> ValidateBatchesInWarehouseAsync(
            IEnumerable<long> batchIds,
            long warehouseId)
        {
            var batches = await _repository.GetBatchesByIdsAsync(batchIds);

            foreach (var batchId in batchIds)
            {
                if (!batches.TryGetValue(batchId, out var batch))
                    throw new BusinessException($"Batch ID {batchId} not found.", "DR002", StatusCodes.Status404NotFound);

                if (batch.WarehouseID != warehouseId)
                    throw new BusinessException(
                        $"Batch ID {batchId} does not belong to warehouse ID {warehouseId}.",
                        "DR003",
                        StatusCodes.Status400BadRequest);
            }

            return batches;
        }

        public async Task<StockTake> ValidateStockTakeCanDestroyAsync(long stockTakeId, long warehouseId)
        {
            var stockTake = await _repository.GetStockTakeByIdAsync(stockTakeId);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "DR011", StatusCodes.Status404NotFound);

            if (stockTake.Status != StockTakeStatus.Completed)
                throw new BusinessException(
                    "Stock take must be completed before creating a destroy receipt.",
                    "DR012",
                    StatusCodes.Status400BadRequest);

            if (!stockTake.IsDestroy)
                throw new BusinessException(
                    "Stock take has no items flagged for destruction.",
                    "DR018",
                    StatusCodes.Status400BadRequest);

            if (stockTake.WarehouseID != warehouseId)
                throw new BusinessException(
                    $"Stock take ID {stockTakeId} does not belong to warehouse ID {warehouseId}.",
                    "DR014",
                    StatusCodes.Status400BadRequest);

            if (await _repository.HasDestroyReceiptAsync(stockTakeId))
                throw new BusinessException(
                    "Stock take already has a destroy receipt.",
                    "DR015",
                    StatusCodes.Status400BadRequest);

            return stockTake;
        }

        public void ValidateStockTakeItemLinksAsync(
            IEnumerable<(long? StockTakeItemID, long BatchID)> items,
            long stockTakeId,
            Dictionary<long, StockTakeItem> stockTakeItems)
        {
            foreach (var (stockTakeItemId, batchId) in items)
            {
                if (!stockTakeItemId.HasValue)
                    continue;

                if (!stockTakeItems.TryGetValue(stockTakeItemId.Value, out var item))
                    throw new BusinessException($"Stock take item ID {stockTakeItemId} not found.", "DR016", StatusCodes.Status404NotFound);

                if (item.StockTakeID != stockTakeId)
                    throw new BusinessException(
                        "Stock take item does not belong to the given stock take.",
                        "DR017",
                        StatusCodes.Status400BadRequest);

                if (item.BatchID != batchId)
                    throw new BusinessException(
                        "Stock take item does not belong to the given batch.",
                        "DR005",
                        StatusCodes.Status400BadRequest);

                if (!item.IsDestroy)
                    throw new BusinessException(
                        $"Stock take item ID {stockTakeItemId} is not flagged for destruction.",
                        "DR019",
                        StatusCodes.Status400BadRequest);
            }
        }

        public void ValidateDestroyQuantity(Batch batch, decimal quantity)
        {
            if (quantity <= 0)
                throw new BusinessException(
                    $"Quantity must be greater than zero for batch ID {batch.BatchID}.",
                    "DR006",
                    StatusCodes.Status400BadRequest);

            if (quantity > batch.QuantityInStock)
                throw new BusinessException(
                    $"Insufficient stock for batch ID {batch.BatchID}. " +
                    $"Available: {batch.QuantityInStock}, requested: {quantity}.",
                    "DR007",
                    StatusCodes.Status400BadRequest);
        }
    }
}
