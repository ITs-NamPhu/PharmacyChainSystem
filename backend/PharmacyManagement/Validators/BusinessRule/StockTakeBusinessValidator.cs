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
    }
}
