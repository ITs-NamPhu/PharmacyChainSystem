namespace PharmacyManagement.DTOs.Dashboard
{
    public class TotalRevenueResponse
    {
        public decimal TotalRevenue { get; set; }
    }

    public class TotalOrdersResponse
    {
        public int TotalOrders { get; set; }
    }

    public class RevenueByBranchItem
    {
        public long BranchID { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RevenueByBranchResponse
    {
        public List<RevenueByBranchItem> Items { get; set; } = new();
    }

    public class NewCustomersResponse
    {
        public int NewCustomers { get; set; }
    }

    public class RevenueTrendPoint
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class RevenueTrendResponse
    {
        public List<RevenueTrendPoint> Items { get; set; } = new();
    }

    public class TopMedicineItem
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopMedicinesResponse
    {
        public List<TopMedicineItem> Items { get; set; } = new();
    }

    public class ExpiringBatchItem
    {
        public long BatchID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public decimal QuantityInStock { get; set; }
        public int DaysUntilExpiry { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }

    public class ExpiringBatchesResponse
    {
        public List<ExpiringBatchItem> Items { get; set; } = new();
    }

    public class LowStockItem
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal QuantityInStock { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }

    public class LowStockResponse
    {
        public List<LowStockItem> Items { get; set; } = new();
    }

    public class DestroyQueueItem
    {
        public long BatchID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string BatchName { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public decimal QuantityInStock { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }

    public class DestroyQueueResponse
    {
        public List<DestroyQueueItem> Items { get; set; } = new();
    }

    public class InventoryCapitalResponse
    {
        public decimal TotalCapital { get; set; }
    }

    public class PendingImportsResponse
    {
        public int PendingCount { get; set; }
    }

    public class StockTransferRequestItem
    {
        public string FromBranch { get; set; } = string.Empty;
        public string ToBranch { get; set; } = string.Empty;
        public string MedicineName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class StockTransferRequestsResponse
    {
        public List<StockTransferRequestItem> Items { get; set; } = new();
    }

    public class PendingStockTransfersResponse
    {
        public int PendingCount { get; set; }
    }

    public class BranchRevenueResponse
    {
        public decimal TotalRevenue { get; set; }
    }

    public class BranchOrdersResponse
    {
        public int TotalOrders { get; set; }
    }

    public class OutOfStockItem
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
    }

    public class OutOfStockResponse
    {
        public List<OutOfStockItem> Items { get; set; } = new();
    }

    public class EmployeeProgressItem
    {
        public long UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
    }

    public class EmployeeProgressResponse
    {
        public List<EmployeeProgressItem> Items { get; set; } = new();
    }

    public class ShiftRevenueResponse
    {
        public decimal TotalRevenue { get; set; }
    }

    public class ShiftOrdersResponse
    {
        public int TotalOrders { get; set; }
    }

    public class PromotionItemResponse
    {
        public long PromotionID { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class PromotionsResponse
    {
        public List<PromotionItemResponse> Items { get; set; } = new();
    }

    public class CounterAlertItem
    {
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal QuantityInStock { get; set; }
    }

    public class CounterAlertsResponse
    {
        public List<CounterAlertItem> Items { get; set; } = new();
    }
}
