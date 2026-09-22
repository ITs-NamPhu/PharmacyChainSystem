using PharmacyManagement.DTOs.AiSearch;
using PharmacyManagement.share;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IAiSearchService
    {
        Task<PagedResult<AiInventoryItem>> SearchInventoryAsync(AiInventoryFilterRequest filter, long? branchScope);
        Task<PagedResult<AiSalesItem>> SearchSalesAsync(AiSalesFilterRequest filter, long? branchScope);
        Task<PagedResult<AiCustomerItem>> SearchCustomersAsync(AiCustomerFilterRequest filter, long? branchScope);
        Task<AiAnalyticsResponse> SearchAnalyticsAsync(AiAnalyticsRequest filter, long? branchScope);
        Task<PagedResult<AiGoodsReceiptItem>> SearchGoodsReceiptsAsync(AiGoodsReceiptFilterRequest filter, long? branchScope);
    }
}