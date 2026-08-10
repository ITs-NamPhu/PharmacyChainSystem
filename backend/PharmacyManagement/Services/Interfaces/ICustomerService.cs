using PharmacyManagement.DTOs.Customer;

namespace PharmacyManagement.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
        Task<CustomerResponse> UpdateAsync(long id, UpdateCustomerRequest request);
        Task DeleteAsync(long id);
        Task<CustomerResponse?> GetByIdAsync(long id);
        Task<CustomerListResponse> GetAllAsync(int page, int count);
        Task<List<CustomerResponse>> GetAllCustomerAsync();
    }
}
