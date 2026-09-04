using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class StockTakeBusinessValidator
    {
        private readonly IStockTakeRepository _repository;

        public StockTakeBusinessValidator(IStockTakeRepository repository)
        {
            _repository = repository;
        }

        public async Task<WareHouse> ValidateWarehouseExistsAsync(long warehouseId)
        {
            var warehouse = await _repository.GetWarehouseByIdAsync(warehouseId);
            if (warehouse == null)
                throw new BusinessException("Warehouse not found.", "ST001", StatusCodes.Status404NotFound);

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
                    throw new BusinessException($"Batch ID {batchId} not found.", "ST002", StatusCodes.Status404NotFound);

                if (batch.WarehouseID != warehouseId)
                    throw new BusinessException(
                        $"Batch ID {batchId} does not belong to warehouse ID {warehouseId}.",
                        "ST003",
                        StatusCodes.Status400BadRequest);
            }

            return batches;
        }

        public void ValidateBalanceEquation(IEnumerable<StockTakeItem> items)
        {
            foreach (var item in items)
            {
                var absDiff = Math.Abs(item.DifferenceQuantity);
                if (item.AdjustQuantity + item.DestroyQuantity != absDiff)
                    throw new BusinessException(
                        $"BatchID {item.BatchID}: AdjustQuantity({item.AdjustQuantity}) + " +
                        $"DestroyQuantity({item.DestroyQuantity}) phải bằng |Diff|({absDiff}).",
                        "ST010",
                        StatusCodes.Status400BadRequest);

                if (item.AdjustQuantity < 0 || item.DestroyQuantity < 0)
                    throw new BusinessException(
                        $"BatchID {item.BatchID}: AdjustQuantity và DestroyQuantity phải >= 0.",
                        "ST011",
                        StatusCodes.Status400BadRequest);
            }
        }
    }
}
