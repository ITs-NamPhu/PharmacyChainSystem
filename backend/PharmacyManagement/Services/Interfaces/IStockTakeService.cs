using PharmacyManagement.DTOs.StockTake;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IStockTakeService
    {
        Task<StockTakeDetailResponse> CreateAsync(CreateStockTakeRequest request, long userId);
        Task<StockTakeDetailResponse> CompleteAsync(long id, long userId);
        Task CancelAsync(long id, long userId);
        Task ApproveAsync(long id, long userId);
        Task<StockTakeDetailResponse?> GetByIdAsync(long id);
        Task<StockTakeListResponse> GetAllAsync(int page, int count, long warehouseId);
    }
}
