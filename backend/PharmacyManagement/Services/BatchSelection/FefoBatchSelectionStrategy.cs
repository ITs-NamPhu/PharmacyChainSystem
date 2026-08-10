using PharmacyManagement.DTOs;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Services.Implements;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.BatchSelection
{
    public class FefoBatchSelectionStrategy : IBatchSelectionStrategy
    {
        public Task<List<BatchAllocation>> ResolveBatchesAsync(
            PreparedInvoiceItem item, long branchId)
        {
            var batches = item.AvailableBatches
                ?? throw new BusinessException(
                    $"No batch available for medicine ID {item.Request.MedicineID}.",
                    "INV007",
                    StatusCodes.Status400BadRequest);

            var allocations = new List<BatchAllocation>();
            decimal remaining = item.BaseUnitQuantity;

            // duyệt từng batches lấy được
            // nếu số lượng trong batch nhỏ hơn remaining thì lấy hết số lượng trong batch -> trừ remain
            // nếu số lượng (remaining) còn lại <= 0 thì break khỏi vòng lặp
            foreach (var batch in batches)
            {
                if (remaining <= 0) break;

                decimal alloc = Math.Min(batch.QuantityInStock, remaining);
                allocations.Add(new BatchAllocation
                {
                    BatchID = batch.BatchID,
                    Quantity = alloc,
                    Batch = batch
                });

                remaining -= alloc;
            }

            // nếu duyệt hết batch mà không đủ số lượng cần lấy thì throw exception
            if (remaining > 0)
                throw new BusinessException(
                    $"Insufficient stock for medicine ID {item.Request.MedicineID}. " +
                    $"Requested: {item.BaseUnitQuantity}, Available: {item.BaseUnitQuantity - remaining}.",
                    "INV008",
                    StatusCodes.Status400BadRequest);

            // trả về batch và số lượng lấy từ batch đó -> để tạo invoice item
            return Task.FromResult(allocations);
        }
    }
}
