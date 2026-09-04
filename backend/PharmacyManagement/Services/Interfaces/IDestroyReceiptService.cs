using PharmacyManagement.DTOs.DestroyReceipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IDestroyReceiptService
    {
        Task<DestroyReceiptDetailResponse> CreateAsync(CreateDestroyReceiptRequest request, long userId);
        Task<DestroyReceiptDetailResponse> UpdateAsync(long id, UpdateDestroyReceiptRequest request, long userId);
        Task<DestroyReceiptDetailResponse> CompleteAsync(long id, long userId);
        Task ApproveAsync(long id, long userId);
        Task RejectAsync(long id, long userId);
        Task DeleteAsync(long id, long userId);
        Task<DestroyReceiptDetailResponse?> GetByIdAsync(long id);
        Task<DestroyReceiptListResponse> GetAllAsync(int page, int count, long warehouseId);
    }
}
