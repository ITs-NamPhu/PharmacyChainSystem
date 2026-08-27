using PharmacyManagement.DTOs.Receipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request, long userId);
        Task<ReceiptResponse?> GetByIdAsync(long id);
        Task<ReceiptListResponse> GetAllAsync(long? customerId, int page, int count);
    }
}
