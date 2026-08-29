using PharmacyManagement.DTOs.Receipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request, long userId);
        Task<ReceiptResponse> UpdateAsync(long id, UpdateReceiptRequest request);
        Task DeleteAsync(long id);
        Task<ReceiptResponse?> GetByIdAsync(long id);
        Task<ReceiptListResponse> GetAllAsync(long? customerId, int page, int count);
    }
}
