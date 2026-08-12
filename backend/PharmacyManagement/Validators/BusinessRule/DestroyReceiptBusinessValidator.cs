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

        public void ValidateStockTakeItemLinksAsync(
            IEnumerable<(long? StockTakeItemID, long BatchID)> items,
            Dictionary<long, StockTakeItem> stockTakeItems)
        {
            foreach (var (stockTakeItemId, batchId) in items)
            {
                if (!stockTakeItemId.HasValue)
                    continue;

                if (!stockTakeItems.TryGetValue(stockTakeItemId.Value, out var item))
                    throw new BusinessException($"Stock take item ID {stockTakeItemId} not found.", "DR004", StatusCodes.Status404NotFound);

                if (item.BatchID != batchId)
                    throw new BusinessException(
                        "Stock take item does not belong to the given batch.",
                        "DR005",
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
