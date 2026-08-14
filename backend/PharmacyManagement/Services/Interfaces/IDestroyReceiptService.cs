using PharmacyManagement.DTOs.DestroyReceipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IDestroyReceiptService
    {
        Task<DestroyReceiptDetailResponse> CreateAsync(CreateDestroyReceiptRequest request, long userId);
        Task ApproveAsync(long id, long userId);
        Task<DestroyReceiptDetailResponse?> GetByIdAsync(long id);
        Task<DestroyReceiptListResponse> GetAllAsync(int page, int count, long warehouseId);
    }
}
