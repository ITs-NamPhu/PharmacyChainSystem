using PharmacyManagement.DTOs.GoodsReceipt;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IGoodsReceiptService
    {
        Task<GoodsReceiptResponse> CreateAsync(CreateGoodsReceiptRequest request, long userId, long branchId);
        Task<GoodsReceiptResponse> UpdateAsync(long id, UpdateGoodsReceiptRequest request, long userId, long branchId);
        Task DeleteAsync(long id, long branchId);
        Task<GoodsReceiptDetailResponse?> GetByIdAsync(long id, long branchId);
        Task<GoodsReceiptListResponse> GetAllAsync(int page, int count, long branchId);
    }
}
