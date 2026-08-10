using PharmacyManagement.DTOs;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Services.Implements;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.BatchSelection
{
    public class ManualBatchSelectionStrategy : IBatchSelectionStrategy
    {
        public Task<List<BatchAllocation>> ResolveBatchesAsync(
            PreparedInvoiceItem item, long branchId)
        {
            var batch = item.ManualBatch
                ?? throw new BusinessException("Batch not found.", "INV002", StatusCodes.Status404NotFound);

            return Task.FromResult(new List<BatchAllocation>
            {
                new BatchAllocation
                {
                    BatchID = batch.BatchID,
                    Quantity = item.BaseUnitQuantity,
                    Batch = batch
                }
            });
        }
    }
}
