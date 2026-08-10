using PharmacyManagement.DTOs.Dashboard;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<TotalRevenueResponse> GetTotalRevenueAsync(long? branchId);
        Task<TotalOrdersResponse> GetTotalOrdersAsync(long? branchId);
        Task<RevenueByBranchResponse> GetRevenueByBranchAsync();
        Task<NewCustomersResponse> GetNewCustomersAsync(long? branchId);
        Task<RevenueTrendResponse> GetRevenueTrendAsync(long? branchId, int days = 7);
        Task<TopMedicinesResponse> GetTopMedicinesAsync(long? branchId, int top = 10);
        Task<ExpiringBatchesResponse> GetExpiringBatchesAsync(long? branchId);
        Task<LowStockResponse> GetLowStockAsync(long? branchId, decimal threshold = 10);
        Task<DestroyQueueResponse> GetDestroyQueueAsync(long? branchId);
        Task<InventoryCapitalResponse> GetInventoryCapitalAsync(long? branchId);
        Task<PendingImportsResponse> GetPendingImportsAsync(long? branchId);
        Task<StockTransferRequestsResponse> GetStockTransferRequestsAsync();
        Task<PendingStockTransfersResponse> GetPendingStockTransfersAsync();
        Task<BranchRevenueResponse> GetBranchRevenueAsync(long branchId);
        Task<BranchOrdersResponse> GetBranchOrdersAsync(long branchId);
        Task<OutOfStockResponse> GetOutOfStockAsync(long branchId);
        Task<EmployeeProgressResponse> GetEmployeeProgressAsync(long branchId);
        Task<ShiftRevenueResponse> GetShiftRevenueAsync(long branchId, long userId);
        Task<ShiftOrdersResponse> GetShiftOrdersAsync(long branchId, long userId);
        Task<PromotionsResponse> GetPromotionsAsync();
        Task<CounterAlertsResponse> GetCounterAlertsAsync(long branchId);
    }
}
