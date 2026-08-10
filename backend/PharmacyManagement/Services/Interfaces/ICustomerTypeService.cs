using PharmacyManagement.DTOs.CustomerType;

namespace PharmacyManagement.Services.Interfaces
{
    public interface ICustomerTypeService
    {
        Task<CustomerTypeResponse> CreateAsync(CreateCustomerTypeRequest request);
        Task<CustomerTypeResponse> UpdateAsync(long id, UpdateCustomerTypeRequest request);
        Task DeleteAsync(long id);
        Task<CustomerTypeResponse?> GetByIdAsync(long id);
        Task<List<CustomerTypeResponse>> GetAllCustomerTypeAsync();
        Task<CustomerTypeListResponse> GetAllAsync(int page, int count);
    }
}
