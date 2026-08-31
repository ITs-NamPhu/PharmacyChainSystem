using PharmacyManagement.DTOs.Receipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IReceiptService
    {
        // Tạo phiếu thu gạch nợ theo FIFO cho khách hàng
        Task<ReceiptResponse> CreateAsync(CreateReceiptRequest request, long userId, long branchId);
        Task<ReceiptResponse> UpdateAsync(long id, UpdateReceiptRequest request);
        Task DeleteAsync(long id);
        Task<ReceiptResponse?> GetByIdAsync(long id);
        Task<ReceiptListResponse> GetAllAsync(long? customerId, int page, int count);
    }
}