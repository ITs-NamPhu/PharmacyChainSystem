using PharmacyManagement.Models;

namespace PharmacyManagement.Extensions
{
    public static class GoodsReceiptQueryExtensions
    {
        private static readonly HashSet<string> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "receiptdate", "receiptnumber", "totalamount"
        };

        public static IQueryable<GoodsReceipt> FilterByBranch(this IQueryable<GoodsReceipt> query, long branchId)
        {
            return query.Where(gr => gr.BranchID == branchId);
        }

        public static IQueryable<GoodsReceipt> FilterByKeyword(this IQueryable<GoodsReceipt> query, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return query;

            keyword = keyword.Trim();

            if (long.TryParse(keyword, out long receiptNumber))
            {
                return query.Where(gr =>
                    gr.ReceiptNumber == receiptNumber ||
                    (gr.Supplier != null && gr.Supplier.SupplierName.Contains(keyword)));
            }

            return query.Where(gr => gr.Supplier != null && gr.Supplier.SupplierName.Contains(keyword));
        }

        public static IQueryable<GoodsReceipt> FilterByDate(this IQueryable<GoodsReceipt> query, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue)
                query = query.Where(gr => gr.ReceiptDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(gr => gr.ReceiptDate < toDate.Value.AddDays(1));

            return query;
        }

        public static IQueryable<GoodsReceipt> FilterBySupplier(this IQueryable<GoodsReceipt> query, long? supplierId)
        {
            return supplierId.HasValue
                ? query.Where(gr => gr.SupplierID == supplierId.Value)
                : query;
        }

        public static IQueryable<GoodsReceipt> FilterByAmount(
            this IQueryable<GoodsReceipt> query, decimal? minAmount, decimal? maxAmount)
        {
            if (minAmount.HasValue)
                query = query.Where(gr => gr.TotalAmount >= minAmount.Value);
            if (maxAmount.HasValue)
                query = query.Where(gr => gr.TotalAmount <= maxAmount.Value);

            return query;
        }

        public static IQueryable<GoodsReceipt> ApplySort(this IQueryable<GoodsReceipt> query, string? sortBy, bool isDescending)
        {
            var column = string.IsNullOrWhiteSpace(sortBy) ? "receiptdate" : sortBy.Trim().ToLower();

            if (!SortableColumns.Contains(column))
                column = "receiptdate";

            return column switch
            {
                "receiptnumber" => isDescending
                    ? query.OrderByDescending(gr => gr.ReceiptNumber)
                    : query.OrderBy(gr => gr.ReceiptNumber),
                "totalamount" => isDescending
                    ? query.OrderByDescending(gr => gr.TotalAmount)
                    : query.OrderBy(gr => gr.TotalAmount),
                _ => isDescending
                    ? query.OrderByDescending(gr => gr.ReceiptDate)
                    : query.OrderBy(gr => gr.ReceiptDate),
            };
        }
    }
}
