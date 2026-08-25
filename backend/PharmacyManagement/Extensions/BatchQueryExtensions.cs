using PharmacyManagement.DTOs.Batch;
using PharmacyManagement.Models;

namespace PharmacyManagement.Extensions
{
    public static class BatchQueryExtensions
    {
        private static readonly HashSet<string> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "expirydate", "quantityinstock", "medicinename"
        };

        public static IQueryable<Batch> FilterByBranch(this IQueryable<Batch> query, long branchId)
        {
            return query.Where(b => b.WareHouse != null && b.WareHouse.BranchID == branchId);
        }

        public static IQueryable<Batch> FilterByWarehouse(this IQueryable<Batch> query, long? warehouseId)
        {
            return warehouseId.HasValue
                ? query.Where(b => b.WarehouseID == warehouseId.Value)
                : query;
        }

        public static IQueryable<Batch> FilterByExpiryStatus(this IQueryable<Batch> query, ExpiryStatus status)
        {
            if (status == ExpiryStatus.All)
                return query;

            var now = DateTime.Now;

            return status switch
            {
                ExpiryStatus.NearExpiry3Months => query.Where(b =>
                    b.QuantityInStock > 0 &&
                    b.ExpiryDate >= now &&
                    b.ExpiryDate <= now.AddMonths(3)),
                ExpiryStatus.NearExpiry6Months => query.Where(b =>
                    b.QuantityInStock > 0 &&
                    b.ExpiryDate > now.AddMonths(3) &&
                    b.ExpiryDate <= now.AddMonths(6)),
                ExpiryStatus.Expired => query.Where(b =>
                    b.QuantityInStock > 0 &&
                    b.ExpiryDate < now),
                _ => query
            };
        }

        public static IQueryable<Batch> FilterByKeyword(this IQueryable<Batch> query, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return query;

            keyword = keyword.Trim();

            return query.Where(b =>
                b.GoodsReceiptItem != null &&
                b.GoodsReceiptItem.Medicine != null &&
                b.GoodsReceiptItem.Medicine.MedicineName.Contains(keyword));
        }

        public static IQueryable<Batch> ApplySort(this IQueryable<Batch> query, string? sortBy, bool isDescending)
        {
            var column = string.IsNullOrWhiteSpace(sortBy) ? "expirydate" : sortBy.Trim().ToLower();

            if (!SortableColumns.Contains(column))
                column = "expirydate";

            return column switch
            {
                "quantityinstock" => isDescending
                    ? query.OrderByDescending(b => b.QuantityInStock)
                    : query.OrderBy(b => b.QuantityInStock),
                "medicinename" => isDescending
                    ? query.OrderByDescending(b => b.GoodsReceiptItem != null && b.GoodsReceiptItem.Medicine != null
                        ? b.GoodsReceiptItem.Medicine.MedicineName : "")
                    : query.OrderBy(b => b.GoodsReceiptItem != null && b.GoodsReceiptItem.Medicine != null
                        ? b.GoodsReceiptItem.Medicine.MedicineName : ""),
                _ => isDescending
                    ? query.OrderByDescending(b => b.ExpiryDate)
                    : query.OrderBy(b => b.ExpiryDate),
            };
        }
    }
}
