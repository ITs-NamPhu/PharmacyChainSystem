using PharmacyManagement.DTOs.Medicine;
using PharmacyManagement.Models;

namespace PharmacyManagement.Extensions
{
    public class MedicineWithStock
    {
        public Medicine Medicine { get; set; } = null!;
        public decimal TotalStock { get; set; }
    }

    public static class MedicineQueryExtensions
    {
        private static readonly HashSet<string> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "medicinename", "retailprice", "wholesaleprice", "totalstock"
        };

        public static IQueryable<MedicineWithStock> AttachStock(
            this IQueryable<Medicine> query, IQueryable<Batch> batches, long branchId)
        {
            var stocks = batches
                .Where(b => b.WareHouse != null && b.WareHouse.BranchID == branchId)
                .GroupBy(b => b.GoodsReceiptItem!.MedicineID)
                .Select(g => new
                {
                    MedicineID = g.Key,
                    TotalStock = g.Sum(b => b.QuantityInStock)
                });

            return from m in query
                   join s in stocks on m.MedicineID equals s.MedicineID into gj
                   from s in gj.DefaultIfEmpty()
                   select new MedicineWithStock
                   {
                       Medicine = m,
                       TotalStock = s != null ? s.TotalStock : 0
                   };
        }

        public static IQueryable<MedicineWithStock> FilterByKeyword(
            this IQueryable<MedicineWithStock> query, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return query;

            keyword = keyword.Trim();
            return query.Where(w => w.Medicine.MedicineName.Contains(keyword));
        }

        public static IQueryable<MedicineWithStock> FilterByCategory(
            this IQueryable<MedicineWithStock> query, long? categoryId)
        {
            return categoryId.HasValue
                ? query.Where(w => w.Medicine.CategoryID == categoryId.Value)
                : query;
        }

        public static IQueryable<MedicineWithStock> FilterByManufacturer(
            this IQueryable<MedicineWithStock> query, long? manufacturerId)
        {
            return manufacturerId.HasValue
                ? query.Where(w => w.Medicine.ManufacturerID == manufacturerId.Value)
                : query;
        }

        public static IQueryable<MedicineWithStock> FilterByStockStatus(
            this IQueryable<MedicineWithStock> query, StockStatus status, decimal? threshold)
        {
            if (status == StockStatus.All)
                return query;

            var t = threshold ?? 10m;

            return status switch
            {
                StockStatus.OutOfStock => query.Where(w => w.TotalStock <= 0),
                StockStatus.LowStock   => query.Where(w => w.TotalStock > 0 && w.TotalStock <= t),
                StockStatus.InStock    => query.Where(w => w.TotalStock > t),
                _ => query
            };
        }

        public static IQueryable<MedicineWithStock> ApplySort(
            this IQueryable<MedicineWithStock> query, string? sortBy, bool isDescending)
        {
            var column = string.IsNullOrWhiteSpace(sortBy) ? "medicinename" : sortBy.Trim().ToLower();

            if (!SortableColumns.Contains(column))
                column = "medicinename";

            return column switch
            {
                "retailprice" => isDescending
                    ? query.OrderByDescending(w => w.Medicine.DefaultRetailPrice)
                    : query.OrderBy(w => w.Medicine.DefaultRetailPrice),
                "wholesaleprice" => isDescending
                    ? query.OrderByDescending(w => w.Medicine.DefaultWholesalePrice)
                    : query.OrderBy(w => w.Medicine.DefaultWholesalePrice),
                "totalstock" => isDescending
                    ? query.OrderByDescending(w => w.TotalStock)
                    : query.OrderBy(w => w.TotalStock),
                _ => isDescending
                    ? query.OrderByDescending(w => w.Medicine.MedicineName)
                    : query.OrderBy(w => w.Medicine.MedicineName),
            };
        }
    }
}
