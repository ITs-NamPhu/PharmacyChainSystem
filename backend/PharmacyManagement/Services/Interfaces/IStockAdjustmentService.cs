using PharmacyManagement.DTOs.StockAdjustment;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IStockAdjustmentService
    {
        Task<StockAdjustmentDetailResponse> CreateAsync(CreateStockAdjustmentRequest request, long userId);
        Task ApproveAsync(long id, long userId);
        Task<StockAdjustmentDetailResponse?> GetByIdAsync(long id);
        Task<StockAdjustmentListResponse> GetAllAsync(int page, int count, long warehouseId);
    }
}
