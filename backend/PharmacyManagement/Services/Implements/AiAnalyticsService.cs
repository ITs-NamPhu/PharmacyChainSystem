using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.AiSearch;

namespace PharmacyManagement.Services.Implements
{
    public partial class AiSearchService
    {
        public async Task<AiAnalyticsResponse> SearchAnalyticsAsync(
            AiAnalyticsRequest filter, long? branchScope)
        {
            var fromDate = filter.FromDate;
            var toDate = filter.ToDate;
            var topN = filter.TopN ?? 10;

            switch (filter.ReportType)
            {
                case "TotalRevenue":
                    return await AnalyticsTotalRevenueAsync(branchScope, fromDate, toDate);
                case "RevenueTrend":
                    return await AnalyticsRevenueTrendAsync(branchScope, fromDate, toDate);
                case "TopSellingMedicine":
                    return await AnalyticsTopSellingMedicineAsync(branchScope, fromDate, toDate, topN);
                case "TopStockMedicine":
                    return await AnalyticsTopStockMedicineAsync(branchScope, topN);
                case "LowStockMedicine":
                    return await AnalyticsLowStockMedicineAsync(branchScope, topN);
                case "TopCustomer":
                    return await AnalyticsTopCustomerAsync(branchScope, fromDate, toDate, byInvoiceCount: false, topN);
                case "TopCustomerByInvoiceCount":
                    return await AnalyticsTopCustomerAsync(branchScope, fromDate, toDate, byInvoiceCount: true, topN);
                case "TopUserByInvoices":
                    return await AnalyticsTopUserByInvoicesAsync(branchScope, fromDate, toDate, topN);
                default:
                    throw new Exceptions.BusinessException(
                        $"ReportType '{filter.ReportType}' khong hop le.",
                        "ANA001");
            }
        }

        private async Task<AiAnalyticsResponse> AnalyticsTotalRevenueAsync(
            long? branchScope, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Invoice.AsNoTracking();
            if (branchScope.HasValue)
                query = query.Where(i => i.BranchID == branchScope.Value);
            if (fromDate.HasValue)
                query = query.Where(i => i.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(i => i.CreatedAt <= toDate.Value);

            var total = await query.SumAsync(i => (decimal?)i.TotalAmount) ?? 0m;

            return new AiAnalyticsResponse
            {
                ReportType = "TotalRevenue",
                FromDate = fromDate,
                ToDate = toDate,
                Items = new List<AiAnalyticsRow>
                {
                    new AiAnalyticsRow { EntityId = 0, EntityName = "Tổng doanh thu", PrimaryMetric = total }
                }
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsRevenueTrendAsync(
            long? branchScope, DateTime? fromDate, DateTime? toDate)
        {
            var from = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            var to = (toDate ?? DateTime.Today).Date;

            var query = _context.Invoice.AsNoTracking()
                .Where(i => i.CreatedAt >= from && i.CreatedAt <= to.AddDays(1));
            if (branchScope.HasValue)
                query = query.Where(i => i.BranchID == branchScope.Value);

            var items = await query
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(i => i.TotalAmount)
                })
                .ToListAsync();

            var byDate = items.ToDictionary(x => x.Date, x => x.Revenue);
            var totalDays = (to - from).Days + 1;

            var rows = Enumerable.Range(0, totalDays)
                .Select(offset =>
                {
                    var day = from.AddDays(offset);
                    return new AiAnalyticsRow
                    {
                        EntityId = 0,
                        EntityName = day.ToString("yyyy-MM-dd"),
                        PrimaryMetric = byDate.TryGetValue(day, out var rev) ? rev : 0m,
                        Date = day
                    };
                })
                .ToList();

            return new AiAnalyticsResponse
            {
                ReportType = "RevenueTrend",
                FromDate = from,
                ToDate = to,
                Items = rows
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsTopSellingMedicineAsync(
            long? branchScope, DateTime? fromDate, DateTime? toDate, int topN)
        {
            var query = _context.InvoiceItem
                .AsNoTracking()
                .Where(ii => ii.Batch != null
                    && ii.Batch.GoodsReceiptItem != null
                    && ii.Batch.GoodsReceiptItem.Medicine != null
                    && ii.Invoice != null);

            if (branchScope.HasValue)
                query = query.Where(ii => ii.Invoice!.BranchID == branchScope.Value);
            if (fromDate.HasValue)
                query = query.Where(ii => ii.Invoice!.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(ii => ii.Invoice!.CreatedAt <= toDate.Value);

            var items = await query
                .GroupBy(ii => new
                {
                    MedicineID = ii.Batch!.GoodsReceiptItem!.MedicineID,
                    MedicineName = ii.Batch.GoodsReceiptItem.Medicine!.MedicineName
                })
                .Select(g => new AiAnalyticsRow
                {
                    EntityId = g.Key.MedicineID,
                    EntityName = g.Key.MedicineName,
                    PrimaryMetric = g.Sum(ii => ii.BaseQuantity),
                    SecondaryMetric = g.Sum(ii => ii.Quantity * ii.UnitPrice)
                })
                .OrderByDescending(r => r.PrimaryMetric)
                .Take(topN)
                .ToListAsync();

            return new AiAnalyticsResponse
            {
                ReportType = "TopSellingMedicine",
                FromDate = fromDate,
                ToDate = toDate,
                Items = items
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsTopStockMedicineAsync(long? branchScope, int topN)
        {
            var query = _context.Batch
                .AsNoTracking()
                .Where(b => b.GoodsReceiptItem != null && b.GoodsReceiptItem.Medicine != null);

            if (branchScope.HasValue)
                query = query.Where(b => b.WareHouse != null && b.WareHouse.BranchID == branchScope.Value);

            var items = await query
                .GroupBy(b => new
                {
                    MedicineID = b.GoodsReceiptItem!.MedicineID,
                    MedicineName = b.GoodsReceiptItem!.Medicine!.MedicineName
                })
                .Select(g => new AiAnalyticsRow
                {
                    EntityId = g.Key.MedicineID,
                    EntityName = g.Key.MedicineName,
                    PrimaryMetric = g.Sum(b => b.QuantityInStock),
                    SecondaryMetric = g.Count()
                })
                .OrderByDescending(r => r.PrimaryMetric)
                .Take(topN)
                .ToListAsync();

            return new AiAnalyticsResponse
            {
                ReportType = "TopStockMedicine",
                Items = items
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsLowStockMedicineAsync(long? branchScope, int topN)
        {
            const decimal threshold = 10m;

            var query = _context.Batch
                .AsNoTracking()
                .Where(b => b.GoodsReceiptItem != null
                    && b.GoodsReceiptItem.Medicine != null
                    && b.QuantityInStock > 0
                    && b.QuantityInStock < threshold);

            if (branchScope.HasValue)
                query = query.Where(b => b.WareHouse != null && b.WareHouse.BranchID == branchScope.Value);

            var items = await query
                .GroupBy(b => new
                {
                    MedicineID = b.GoodsReceiptItem!.MedicineID,
                    MedicineName = b.GoodsReceiptItem!.Medicine!.MedicineName
                })
                .Select(g => new AiAnalyticsRow
                {
                    EntityId = g.Key.MedicineID,
                    EntityName = g.Key.MedicineName,
                    PrimaryMetric = g.Sum(b => b.QuantityInStock),
                    SecondaryMetric = g.Count()
                })
                .OrderBy(r => r.PrimaryMetric)
                .Take(topN)
                .ToListAsync();

            return new AiAnalyticsResponse
            {
                ReportType = "LowStockMedicine",
                Items = items
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsTopCustomerAsync(
            long? branchScope, DateTime? fromDate, DateTime? toDate, bool byInvoiceCount, int topN)
        {
            var query = _context.Invoice
                .AsNoTracking()
                .Where(i => i.Customer != null);

            if (branchScope.HasValue)
                query = query.Where(i => i.BranchID == branchScope.Value);
            if (fromDate.HasValue)
                query = query.Where(i => i.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(i => i.CreatedAt <= toDate.Value);

            var grouped = query
                .GroupBy(i => new
                {
                    i.CustomerID,
                    i.Customer!.CustomerName
                })
                .Select(g => new AiAnalyticsRow
                {
                    EntityId = g.Key.CustomerID,
                    EntityName = g.Key.CustomerName,
                    PrimaryMetric = g.Sum(i => i.TotalAmount),
                    SecondaryMetric = g.Count()
                });

            var items = byInvoiceCount
                ? await grouped.OrderByDescending(r => r.SecondaryMetric).Take(topN).ToListAsync()
                : await grouped.OrderByDescending(r => r.PrimaryMetric).Take(topN).ToListAsync();

            return new AiAnalyticsResponse
            {
                ReportType = byInvoiceCount ? "TopCustomerByInvoiceCount" : "TopCustomer",
                FromDate = fromDate,
                ToDate = toDate,
                Items = items
            };
        }

        private async Task<AiAnalyticsResponse> AnalyticsTopUserByInvoicesAsync(
            long? branchScope, DateTime? fromDate, DateTime? toDate, int topN)
        {
            var query = _context.Invoice
                .AsNoTracking()
                .Where(i => i.User != null);

            if (branchScope.HasValue)
                query = query.Where(i => i.BranchID == branchScope.Value);
            if (fromDate.HasValue)
                query = query.Where(i => i.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(i => i.CreatedAt <= toDate.Value);

            var items = await query
                .GroupBy(i => new
                {
                    i.UserID,
                    i.User!.FullName
                })
                .Select(g => new AiAnalyticsRow
                {
                    EntityId = g.Key.UserID,
                    EntityName = g.Key.FullName,
                    PrimaryMetric = g.Count(),
                    SecondaryMetric = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(r => r.PrimaryMetric)
                .Take(topN)
                .ToListAsync();

            return new AiAnalyticsResponse
            {
                ReportType = "TopUserByInvoices",
                FromDate = fromDate,
                ToDate = toDate,
                Items = items
            };
        }
    }
}