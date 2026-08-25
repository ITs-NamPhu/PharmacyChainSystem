using PharmacyManagement.DTOs.Invoice;
using PharmacyManagement.DTOs.Batch;
using PharmacyManagement.DTOs.UserByBranch;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, long userId, long branchId);
        Task<InvoiceResponse> UpdateAsync(long id, UpdateInvoiceRequest request, long userId, long branchId);
        Task DeleteAsync(long id, long branchId);
        Task<InvoiceDetailResponse?> GetByIdAsync(long id, long branchId);
        Task<InvoiceListResponse> GetAllAsync(InvoiceFilterDto filter, long branchId);
        Task<List<BatchByMedicineResponse>> GetBatchesByMedicineAsync(long medicineID, long branchID);
        Task<FefoResultResponse> GetFefoBatchesAsync(long medicineID, decimal quantity, long branchID);
        Task<List<UserByBranchResponse>> GetUsersByBranchAsync(long branchID);
    }
}
