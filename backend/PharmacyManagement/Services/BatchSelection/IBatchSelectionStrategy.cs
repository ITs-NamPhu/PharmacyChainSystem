using PharmacyManagement.DTOs;
using PharmacyManagement.Services.Implements;

namespace PharmacyManagement.Services.BatchSelection
{
    public interface IBatchSelectionStrategy
    {
        Task<List<BatchAllocation>> ResolveBatchesAsync(PreparedInvoiceItem item, long branchId);
    }
}
