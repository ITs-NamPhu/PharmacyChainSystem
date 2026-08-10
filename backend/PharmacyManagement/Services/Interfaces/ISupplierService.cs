using PharmacyManagement.DTOs.Supplier;

namespace PharmacyManagement.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<SupplierResponse> CreateAsync(CreateSupplierRequest request);
        Task<SupplierResponse> UpdateAsync(long id, UpdateSupplierRequest request);
        Task DeleteAsync(long id);
        Task<SupplierResponse?> GetByIdAsync(long id);
        Task<SupplierListResponse> GetAllAsync(int page, int count);
    }
}
