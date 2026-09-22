using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.AiSearch;
using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;

namespace PharmacyManagement.Services.Implements
{
    public partial class AiSearchService : IAiSearchService
    {
        private readonly PharmacySystemDbContext _context;

        public AiSearchService(PharmacySystemDbContext context)
        {
            _context = context;
        }

        // ============================= INVENTORY =============================

        public async Task<PagedResult<AiInventoryItem>> SearchInventoryAsync(
            AiInventoryFilterRequest filter, long? branchScope)
        {
            // Phạm vi thật luôn theo header/role. BranchId từ body chỉ dùng khi
            // scope rỗng (admin/manage_supply đang xem toàn hệ thống).
            long? branchId = branchScope ?? filter.BranchId;
            var threshold = filter.StockThreshold ?? 10m;
            var now = DateTime.Now;

            var query = _context.Batch
                .AsNoTracking()
                .Where(b => b.GoodsReceiptItem != null && b.GoodsReceiptItem.Medicine != null);

            query = ApplyInventoryFilters(query, filter, branchId, threshold, now);

            var numRecords = await query.CountAsync();

            var items = await query
                .OrderBy(b => b.QuantityInStock)
                .ThenBy(b => b.ExpiryDate)
                .Skip(PaginationHelper.GetSkip(filter.Page!.Value, filter.Count!.Value))
                .Take(filter.Count!.Value)
                .Select(b => new AiInventoryItem
                {
                    MedicineId = b.GoodsReceiptItem!.MedicineID,
                    MedicineName = b.GoodsReceiptItem!.Medicine!.MedicineName,
                    UnitName = b.GoodsReceiptItem.UnitName,
                    CategoryName = b.GoodsReceiptItem.Medicine.MedicineCategory != null
                        ? b.GoodsReceiptItem.Medicine.MedicineCategory.CategoryName
                        : null,
                    ManufacturerName = b.GoodsReceiptItem.Medicine.ManuFacturer != null
                        ? b.GoodsReceiptItem.Medicine.ManuFacturer.ManufacturerName
                        : null,
                    BatchID = b.BatchID,
                    BatchName = "Lô " + b.BatchID,
                    QuantityReceived = b.QuantityReceived,
                    QuantityInStock = b.QuantityInStock,
                    ExpiryDate = b.ExpiryDate,
                    Price = b.GoodsReceiptItem.Medicine.DefaultRetailPrice,
                    WarehouseName = b.WareHouse != null ? b.WareHouse.WarehouseName : null,
                    BranchName = b.WareHouse != null && b.WareHouse.Branch != null
                        ? b.WareHouse.Branch.BranchName
                        : null
                })
                .ToListAsync();

            return new PagedResult<AiInventoryItem>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = PaginationHelper.GetTotalPage(numRecords, filter.Count.Value)
            };
        }

        private static IQueryable<Batch> ApplyInventoryFilters(
            IQueryable<Batch> query, AiInventoryFilterRequest filter,
            long? branchId, decimal threshold, DateTime now)
        {
            if (branchId.HasValue)
                query = query.Where(b => b.WareHouse != null && b.WareHouse.BranchID == branchId.Value);

            if (filter.WareHouseId.HasValue)
                query = query.Where(b => b.WarehouseID == filter.WareHouseId.Value);

            if (filter.MedicineId.HasValue)
                query = query.Where(b => b.GoodsReceiptItem!.MedicineID == filter.MedicineId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(b =>
                    b.GoodsReceiptItem!.Medicine!.MedicineName.Contains(kw)
                    || (b.GoodsReceiptItem.Medicine.ManuFacturer != null
                        && b.GoodsReceiptItem.Medicine.ManuFacturer.ManufacturerName.Contains(kw)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(b => b.GoodsReceiptItem!.Medicine!.CategoryID == filter.CategoryId.Value);

            if (filter.ManufacturerId.HasValue)
                query = query.Where(b => b.GoodsReceiptItem!.Medicine!.ManufacturerID == filter.ManufacturerId.Value);

            if (filter.InStock == true)
                query = query.Where(b => b.QuantityInStock > 0);

            if (filter.OutOfStock == true)
                query = query.Where(b => b.QuantityInStock == 0);

            if (filter.IsLowStock == true)
                query = query.Where(b => b.QuantityInStock > 0 && b.QuantityInStock < threshold);

            if (filter.IsExpiringSoon == true)
            {
                var from = now.AddMonths(3);
                var to = now.AddMonths(6);
                query = query.Where(b => b.ExpiryDate >= from && b.ExpiryDate <= to && b.QuantityInStock > 0);
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(b => b.GoodsReceiptItem!.Medicine!.DefaultRetailPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(b => b.GoodsReceiptItem!.Medicine!.DefaultRetailPrice <= filter.MaxPrice.Value);

            return query;
        }

        // ============================= SALES =============================

        public async Task<PagedResult<AiSalesItem>> SearchSalesAsync(
            AiSalesFilterRequest filter, long? branchScope)
        {
            long? branchId = branchScope ?? filter.BranchId;

            // Vật lý hóa danh sách lô đã phát sinh trả hàng để dùng lại trong filter & projection
            List<long> returnedBatchIds = await _context.SalesReturnItem
                .Select(s => s.BatchID)
                .Distinct()
                .ToListAsync();

            var query = _context.Invoice
                .AsNoTracking()
                .Where(i => i.Customer != null);

            if (branchId.HasValue)
                query = query.Where(i => i.BranchID == branchId.Value);

            if (filter.InvoiceId.HasValue)
                query = query.Where(i => i.InvoiceID == filter.InvoiceId.Value);

            if (filter.CustomerId.HasValue)
                query = query.Where(i => i.CustomerID == filter.CustomerId.Value);

            if (filter.UserId.HasValue)
                query = query.Where(i => i.UserID == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.CustomerPhone))
            {
                var phone = filter.CustomerPhone.Trim();
                query = query.Where(i => i.Customer!.Phone != null && i.Customer.Phone.Contains(phone));
            }

            if (filter.FromDate.HasValue)
                query = query.Where(i => i.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(i => i.CreatedAt <= filter.ToDate.Value);

            if (filter.MinTotal.HasValue)
                query = query.Where(i => i.TotalAmount >= filter.MinTotal.Value);

            if (filter.MaxTotal.HasValue)
                query = query.Where(i => i.TotalAmount <= filter.MaxTotal.Value);

            if (filter.PaymentStatus.HasValue)
                query = query.Where(i => i.PaymentStatus == filter.PaymentStatus.Value);

            if (filter.HasReturnedItems == true)
                query = query.Where(i => i.InvoiceItem != null
                    && i.InvoiceItem.Any(ii => returnedBatchIds.Contains(ii.BatchID)));

            var numRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip(PaginationHelper.GetSkip(filter.Page!.Value, filter.Count!.Value))
                .Take(filter.Count!.Value)
                .Select(i => new AiSalesItem
                {
                    InvoiceId = i.InvoiceID,
                    CreatedAt = i.CreatedAt,
                    CustomerName = i.Customer!.CustomerName,
                    CustomerPhone = i.Customer.Phone,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    Outstanding = i.TotalAmount - i.PaidAmount,
                    PaymentStatus = i.PaymentStatus,
                    SellerName = i.User != null ? i.User.FullName : null,
                    HasReturnedItems = i.InvoiceItem != null
                        && i.InvoiceItem.Any(ii => returnedBatchIds.Contains(ii.BatchID))
                })
                .ToListAsync();

            return new PagedResult<AiSalesItem>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = PaginationHelper.GetTotalPage(numRecords, filter.Count.Value)
            };
        }

        // ============================= CUSTOMER / DEBT =============================

        public async Task<PagedResult<AiCustomerItem>> SearchCustomersAsync(
            AiCustomerFilterRequest filter, long? branchScope)
        {
            var query = _context.Customer
                .AsNoTracking()
                .Where(c => c.CustomerType != null);

            if (filter.CustomerId.HasValue)
                query = query.Where(c => c.CustomerID == filter.CustomerId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(c =>
                    c.CustomerName.Contains(kw)
                    || (c.Phone != null && c.Phone.Contains(kw)));
            }

            if (filter.CustomerTypeId.HasValue)
                query = query.Where(c => c.CustomerTypeID == filter.CustomerTypeId.Value);

            if (filter.MinWalletBalance.HasValue)
                query = query.Where(c => c.WalletBalance >= filter.MinWalletBalance.Value);

            if (filter.MaxWalletBalance.HasValue)
                query = query.Where(c => c.WalletBalance <= filter.MaxWalletBalance.Value);

            if (filter.HasActiveDebt == true)
            {
                var debtInvoices = _context.Invoice.AsNoTracking();
                if (branchScope.HasValue)
                    debtInvoices = debtInvoices.Where(i => i.BranchID == branchScope.Value);

                query = query.Where(c => debtInvoices.Any(i =>
                    i.CustomerID == c.CustomerID && i.PaidAmount < i.TotalAmount));
            }

            var numRecords = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.CustomerName)
                .Skip(PaginationHelper.GetSkip(filter.Page!.Value, filter.Count!.Value))
                .Take(filter.Count!.Value)
                .Select(c => new AiCustomerItem
                {
                    CustomerId = c.CustomerID,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Address = c.Address,
                    Email = c.Email,
                    CustomerTypeName = c.CustomerType!.TypeName,
                    WalletBalance = c.WalletBalance,
                    DebtAmount = c.Invoice!
                        .Where(i => i.PaidAmount < i.TotalAmount
                                    && (!branchScope.HasValue || i.BranchID == branchScope.Value))
                        .Sum(i => (decimal?)(i.TotalAmount - i.PaidAmount)) ?? 0m
                })
                .ToListAsync();

            return new PagedResult<AiCustomerItem>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = PaginationHelper.GetTotalPage(numRecords, filter.Count.Value)
            };
        }

        // ============================= GOODS RECEIPT (NHẬP HÀNG) =============================

        public async Task<PagedResult<AiGoodsReceiptItem>> SearchGoodsReceiptsAsync(
            AiGoodsReceiptFilterRequest filter, long? branchScope)
        {
            long? branchId = branchScope ?? filter.BranchId;

            var query = _context.GoodsReceipt
                .AsNoTracking()
                .Where(g => g.Supplier != null);

            if (branchId.HasValue)
                query = query.Where(g => g.BranchID == branchId.Value);

            if (filter.GoodsReceiptId.HasValue)
                query = query.Where(g => g.GoodsReceiptID == filter.GoodsReceiptId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim();
                query = query.Where(g =>
                    g.Supplier!.SupplierName.Contains(kw)
                    || g.ReceiptNumber.ToString().Contains(kw));
            }

            if (filter.SupplierId.HasValue)
                query = query.Where(g => g.SupplierID == filter.SupplierId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(g => g.ReceiptDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(g => g.ReceiptDate <= filter.ToDate.Value);

            if (filter.Status != null && !string.IsNullOrWhiteSpace(filter.Status))
            {
                if (!Enum.TryParse<StatusTicket>(filter.Status.Trim(), ignoreCase: true, out var status))
                    throw new Exceptions.BusinessException(
                        $"Status '{filter.Status}' khong hop le. Chi chap nhan: PENDING | APPROVED | COMPLETE | REJECTED.",
                        "ANA002");
                query = query.Where(g => g.Status == status);
            }

            if (filter.MinTotal.HasValue)
                query = query.Where(g => g.TotalAmount >= filter.MinTotal.Value);

            if (filter.MaxTotal.HasValue)
                query = query.Where(g => g.TotalAmount <= filter.MaxTotal.Value);

            var numRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(g => g.ReceiptDate)
                .Skip(PaginationHelper.GetSkip(filter.Page!.Value, filter.Count!.Value))
                .Take(filter.Count!.Value)
                .Select(g => new AiGoodsReceiptItem
                {
                    GoodsReceiptId = g.GoodsReceiptID,
                    ReceiptNumber = g.ReceiptNumber,
                    SupplierName = g.Supplier!.SupplierName,
                    UserName = g.User != null ? g.User.FullName : string.Empty,
                    ReceiptDate = g.ReceiptDate,
                    TotalAmount = g.TotalAmount,
                    PaidAmount = g.PaidAmount,
                    Note = g.Note ?? string.Empty,
                    Status = g.Status.ToString(),
                    ApprovedBy = g.ApprovedBy,
                    ApprovedAt = g.ApprovedAt
                })
                .ToListAsync();

            return new PagedResult<AiGoodsReceiptItem>
            {
                Items = items,
                NumRecords = numRecords,
                TotalPage = PaginationHelper.GetTotalPage(numRecords, filter.Count.Value)
            };
        }

        // ============================= ANALYTICS / REPORT =============================
        // Xem AiAnalyticsService.cs (partial class) cho SearchAnalyticsAsync và các nhánh báo cáo.
    }
}