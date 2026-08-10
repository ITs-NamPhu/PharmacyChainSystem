using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.Dashboard;
using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;
using System.Diagnostics;

namespace PharmacyManagement.Services.Implements
{
    public class DashboardService : IDashboardService
    {
        private readonly PharmacySystemDbContext _context;

        public DashboardService(PharmacySystemDbContext context)
        {
            _context = context;
        }

        // lấy tổng doanh thu của 1 branch
        public async Task<TotalRevenueResponse> GetTotalRevenueAsync(long? branchId)
        {
            var query = _context.Invoice.AsQueryable();
            if (branchId.HasValue)
                query = query.Where(i => i.BranchID == branchId.Value);

            var total = await query.SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
            Debug.WriteLine("**************************************");
            Debug.WriteLine(total);
            return new TotalRevenueResponse { TotalRevenue = total };
        }

        // lấy các đơn hàng
        public async Task<TotalOrdersResponse> GetTotalOrdersAsync(long? branchId)
        {
            var query = _context.Invoice.AsQueryable();
            if (branchId.HasValue)
                query = query.Where(i => i.BranchID == branchId.Value);

            var count = await query.CountAsync();
            return new TotalOrdersResponse { TotalOrders = count };
        }

        // lấy tổng doanh thu theo branch và tỷ trọng %
        public async Task<RevenueByBranchResponse> GetRevenueByBranchAsync()
        {
            var items = await _context.Invoice
                .GroupBy(i => new { i.BranchID, i.Branch!.BranchName })
                .Select(g => new
                {
                    g.Key.BranchID,
                    g.Key.BranchName,
                    Revenue = g.Sum(i => i.TotalAmount)
                })
                .ToListAsync();

            var totalRevenue = items.Sum(i => i.Revenue);

            return new RevenueByBranchResponse
            {
                Items = items.Select(i => new RevenueByBranchItem
                {
                    BranchID = i.BranchID,
                    BranchName = i.BranchName,
                    Revenue = i.Revenue,
                    Percentage = totalRevenue > 0 ? Math.Round(i.Revenue / totalRevenue * 100, 1) : 0
                }).ToList()
            };
        }

        // lấy ds khách mới
        public async Task<NewCustomersResponse> GetNewCustomersAsync(long? branchId)
        {
            var query = _context.Customer.AsQueryable();
            if (branchId.HasValue)
            {
                query = query.Where(c => c.Invoice!.Any(i => i.BranchID == branchId.Value));
            }

            var count = await query.CountAsync();
            return new NewCustomersResponse { NewCustomers = count };
        }


        // lấy doanh thu theo ngày trong 7 ngày gần nhất
        public async Task<RevenueTrendResponse> GetRevenueTrendAsync(long? branchId, int days = 7)
        {
            var fromDate = DateTime.Now.AddDays(-days).Date;
            var query = _context.Invoice
                .Where(i => i.CreatedAt >= fromDate);

            if (branchId.HasValue)
                query = query.Where(i => i.BranchID == branchId.Value);

            var items = await query
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new RevenueTrendPoint
                {
                    Date = g.Key,
                    Revenue = g.Sum(i => i.TotalAmount)
                })
                .OrderBy(p => p.Date)
                .ToListAsync();

            var allDates = Enumerable.Range(0, days)
                .Select(offset => fromDate.AddDays(offset))
                .ToList();

            var result = allDates.GroupJoin(items,
                d => d,
                p => p.Date,
                (d, ps) => new RevenueTrendPoint
                {
                    Date = d,
                    Revenue = ps.FirstOrDefault()?.Revenue ?? 0
                }).OrderBy(p => p.Date).ToList();

            return new RevenueTrendResponse { Items = result };
        }


        // lấy ds top 10 thuốc bán chạy nhất
        public async Task<TopMedicinesResponse> GetTopMedicinesAsync(long? branchId, int top = 10)
        {
            var query = _context.InvoiceItem
                .Include(ii => ii.Batch!)
                    .ThenInclude(b => b.GoodsReceiptItem!)
                        .ThenInclude(gri => gri.Medicine)
                .Include(ii => ii.Invoice)
                .AsQueryable();

            if (branchId.HasValue)
                query = query.Where(ii => ii.Invoice!.BranchID == branchId.Value);

            var items = await query
                .GroupBy(ii => new
                {
                    MedicineID = ii.Batch!.GoodsReceiptItem!.MedicineID,
                    MedicineName = ii.Batch.GoodsReceiptItem.Medicine!.MedicineName
                })
                .Select(g => new TopMedicineItem
                {
                    MedicineID = g.Key.MedicineID,
                    MedicineName = g.Key.MedicineName,
                    TotalQuantity = g.Sum(ii => ii.BaseQuantity),
                    TotalRevenue = g.Sum(ii => ii.Quantity * ii.UnitPrice)
                })
                .OrderByDescending(m => m.TotalQuantity)
                .Take(top)
                .ToListAsync();

            return new TopMedicinesResponse { Items = items };
        }

        // lấy batch sắp hết hạn trong 3 đến 6 tháng tới
        public async Task<ExpiringBatchesResponse> GetExpiringBatchesAsync(long? branchId)
        {
            var threeMonthsLater = DateTime.Now.AddMonths(3);
            var sixMonthsLater = DateTime.Now.AddMonths(6);

            var query = _context.Batch
                .Include(b => b.WareHouse!.Branch)
                .Include(b => b.GoodsReceiptItem!.Medicine)
                .Where(b => b.ExpiryDate >= threeMonthsLater
                         && b.ExpiryDate <= sixMonthsLater
                         && b.QuantityInStock > 0);

            if (branchId.HasValue)
                query = query.Where(b => b.WareHouse!.BranchID == branchId.Value);

            var items = await query
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new ExpiringBatchItem
                {
                    BatchID = b.BatchID,
                    MedicineName = b.GoodsReceiptItem!.Medicine!.MedicineName,
                    BatchName = "Lô " + b.BatchID,
                    ExpiryDate = b.ExpiryDate,
                    QuantityInStock = b.QuantityInStock,
                    DaysUntilExpiry = (int)(b.ExpiryDate - DateTime.Now).TotalDays,
                    BranchName = b.WareHouse!.Branch!.BranchName
                })
                .ToListAsync();

            return new ExpiringBatchesResponse { Items = items };
        }


        // báo động tồn kho dưới 10 đơn vị tính
        public async Task<LowStockResponse> GetLowStockAsync(long? branchId, decimal threshold = 10)
        {
            var query = _context.Batch
                .Include(b => b.GoodsReceiptItem!.Medicine)
                .Include(b => b.WareHouse!.Branch)
                .Where(b => b.QuantityInStock > 0 && b.QuantityInStock < threshold);

            if (branchId.HasValue)
                query = query.Where(b => b.WareHouse!.BranchID == branchId.Value);

            var items = await query
                .GroupBy(b => new { b.GoodsReceiptItem!.MedicineID, MedicineName = b.GoodsReceiptItem.Medicine!.MedicineName, b.WareHouse!.Branch!.BranchName })
                .Select(g => new LowStockItem
                {
                    MedicineID = g.Key.MedicineID,
                    MedicineName = g.Key.MedicineName,
                    QuantityInStock = g.Sum(b => b.QuantityInStock),
                    BranchName = g.Key.BranchName
                })
                .OrderBy(i => i.QuantityInStock)
                .ToListAsync();

            return new LowStockResponse { Items = items };
        }

        // lấy ds các thuốc chờ bị hủy (hết hạn nhưng còn tồn kho)
        public async Task<DestroyQueueResponse> GetDestroyQueueAsync(long? branchId)
        {
            var now = DateTime.Now;
            var query = _context.Batch
                .Include(b => b.GoodsReceiptItem!.Medicine)
                .Include(b => b.WareHouse!.Branch)
                .Where(b => b.ExpiryDate < now && b.QuantityInStock > 0);

            if (branchId.HasValue)
                query = query.Where(b => b.WareHouse!.BranchID == branchId.Value);

            var items = await query
                .OrderBy(b => b.ExpiryDate)
                .Select(b => new DestroyQueueItem
                {
                    BatchID = b.BatchID,
                    MedicineName = b.GoodsReceiptItem!.Medicine!.MedicineName,
                    BatchName = "Lô " + b.BatchID,
                    ExpiryDate = b.ExpiryDate,
                    QuantityInStock = b.QuantityInStock,
                    BranchName = b.WareHouse!.Branch!.BranchName
                })
                .ToListAsync();

            return new DestroyQueueResponse { Items = items };
        }

        // lấy giá trị(tiền) tồn kho
        public async Task<InventoryCapitalResponse> GetInventoryCapitalAsync(long? branchId)
        {
            var query = _context.Batch
                .Include(b => b.GoodsReceiptItem)
                .Include(b => b.WareHouse)
                .Where(b => b.QuantityInStock > 0);

            if (branchId.HasValue)
                query = query.Where(b => b.WareHouse!.BranchID == branchId.Value);

            var total = await query.SumAsync(b => b.QuantityInStock * (b.GoodsReceiptItem!.UnitCost));
            return new InventoryCapitalResponse { TotalCapital = total };
        }

        // lấy ds các đơn nhập hàng đang chờ xử lý
        public async Task<PendingImportsResponse> GetPendingImportsAsync(long? branchId)
        {
            var query = _context.GoodsReceipt.AsQueryable();
            if (branchId.HasValue)
                query = query.Where(g => g.BranchID == branchId.Value);

            var count = await query.CountAsync();
            return new PendingImportsResponse { PendingCount = count };
        }


        public Task<StockTransferRequestsResponse> GetStockTransferRequestsAsync()
        {
            return Task.FromResult(new StockTransferRequestsResponse { Items = new List<StockTransferRequestItem>() });
        }

        public Task<PendingStockTransfersResponse> GetPendingStockTransfersAsync()
        {
            return Task.FromResult(new PendingStockTransfersResponse { PendingCount = 0 });
        }

        // lấy doanh thu theo chi nhánh
        public async Task<BranchRevenueResponse> GetBranchRevenueAsync(long branchId)
        {
            var total = await _context.Invoice
                .Where(i => i.BranchID == branchId)
                .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
            return new BranchRevenueResponse { TotalRevenue = total };
        }

        public async Task<BranchOrdersResponse> GetBranchOrdersAsync(long branchId)
        {
            var count = await _context.Invoice
                .Where(i => i.BranchID == branchId)
                .CountAsync();
            return new BranchOrdersResponse { TotalOrders = count };
        }

        public async Task<OutOfStockResponse> GetOutOfStockAsync(long branchId)
        {
            var warehouseIds = await _context.WareHouse
                .Where(w => w.BranchID == branchId)
                .Select(w => w.WarehouseID)
                .ToListAsync();

            var medicineIdsWithStock = await _context.Batch
                .Where(b => warehouseIds.Contains(b.WarehouseID) && b.QuantityInStock > 0)
                .Select(b => b.GoodsReceiptItem!.MedicineID)
                .Distinct()
                .ToListAsync();

            var allMedicineIds = await _context.Medicine
                .Select(m => m.MedicineID)
                .ToListAsync();

            var outOfStockIds = allMedicineIds.Except(medicineIdsWithStock).ToList();

            var items = await _context.Medicine
                .Where(m => outOfStockIds.Contains(m.MedicineID))
                .Select(m => new OutOfStockItem
                {
                    MedicineID = m.MedicineID,
                    MedicineName = m.MedicineName
                })
                .ToListAsync();

            return new OutOfStockResponse { Items = items };
        }

        public async Task<EmployeeProgressResponse> GetEmployeeProgressAsync(long branchId)
        {
            var today = DateTime.Now.Date;
            var items = await _context.Invoice
                .Where(i => i.BranchID == branchId && i.CreatedAt >= today)
                .GroupBy(i => new { i.UserID, i.User!.FullName })
                .Select(g => new EmployeeProgressItem
                {
                    UserID = g.Key.UserID,
                    UserName = g.Key.FullName,
                    TotalRevenue = g.Sum(i => i.TotalAmount),
                    TotalOrders = g.Count()
                })
                .OrderByDescending(e => e.TotalRevenue)
                .ToListAsync();

            return new EmployeeProgressResponse { Items = items };
        }

        public async Task<ShiftRevenueResponse> GetShiftRevenueAsync(long branchId, long userId)
        {
            var today = DateTime.Now.Date;
            var total = await _context.Invoice
                .Where(i => i.BranchID == branchId && i.UserID == userId && i.CreatedAt >= today)
                .SumAsync(i => (decimal?)i.TotalAmount) ?? 0;
            return new ShiftRevenueResponse { TotalRevenue = total };
        }

        public async Task<ShiftOrdersResponse> GetShiftOrdersAsync(long branchId, long userId)
        {
            var today = DateTime.Now.Date;
            var count = await _context.Invoice
                .Where(i => i.BranchID == branchId && i.UserID == userId && i.CreatedAt >= today)
                .CountAsync();
            return new ShiftOrdersResponse { TotalOrders = count };
        }

        public async Task<PromotionsResponse> GetPromotionsAsync()
        {
            var now = DateTime.Now;
            var items = await _context.Promotion
                .Where(p => p.StartDate <= now && p.EndDate >= now)
                .Select(p => new PromotionItemResponse
                {
                    PromotionID = p.PromotionID,
                    PromotionName = p.PromotionName,
                    Description = p.PromotionName,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .ToListAsync();

            return new PromotionsResponse { Items = items };
        }

        public async Task<CounterAlertsResponse> GetCounterAlertsAsync(long branchId)
        {
            var threshold = 5;
            var warehouseIds = await _context.WareHouse
                .Where(w => w.BranchID == branchId)
                .Select(w => w.WarehouseID)
                .ToListAsync();

            var items = await _context.Batch
                .Include(b => b.GoodsReceiptItem!.Medicine)
                .Where(b => warehouseIds.Contains(b.WarehouseID)
                         && b.QuantityInStock > 0
                         && b.QuantityInStock < threshold)
                .GroupBy(b => new { b.GoodsReceiptItem!.MedicineID, b.GoodsReceiptItem.Medicine!.MedicineName })
                .Select(g => new CounterAlertItem
                {
                    MedicineID = g.Key.MedicineID,
                    MedicineName = g.Key.MedicineName,
                    QuantityInStock = g.Sum(b => b.QuantityInStock)
                })
                .OrderBy(i => i.QuantityInStock)
                .ToListAsync();

            return new CounterAlertsResponse { Items = items };
        }
    }
}
