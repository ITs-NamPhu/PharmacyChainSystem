using PharmacyManagement.DTOs.StockAdjustment;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IStockAdjustmentService
    {
        Task<StockAdjustmentDetailResponse> CreateAsync(CreateStockAdjustmentRequest request, long userId);
        Task<StockAdjustmentDetailResponse> UpdateAsync(long id, UpdateStockAdjustmentRequest request, long userId);
        Task<StockAdjustmentDetailResponse> CompleteAsync(long id, long userId);
        Task ApproveAsync(long id, long userId);
        Task RejectAsync(long id, long userId);
        Task DeleteAsync(long id, long userId);
        Task<StockAdjustmentDetailResponse?> GetByIdAsync(long id);
        Task<StockAdjustmentListResponse> GetAllAsync(int page, int count, long warehouseId);
    }
}
