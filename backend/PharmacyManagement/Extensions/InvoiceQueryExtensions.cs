using PharmacyManagement.Models;

namespace PharmacyManagement.Extensions
{
    public static class InvoiceQueryExtensions
    {
        private static readonly HashSet<string> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "createdat", "totalamount", "paidamount"
        };

        public static IQueryable<Invoice> FilterByBranch(this IQueryable<Invoice> query, long branchId)
        {
            return query.Where(i => i.BranchID == branchId);
        }

        public static IQueryable<Invoice> FilterByKeyword(this IQueryable<Invoice> query, string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return query;

            keyword = keyword.Trim();

            if (long.TryParse(keyword, out long invoiceId))
            {
                return query.Where(i =>
                    i.InvoiceID == invoiceId ||
                    (i.Customer != null && i.Customer.CustomerName.Contains(keyword)));
            }

            return query.Where(i => i.Customer != null && i.Customer.CustomerName.Contains(keyword));
        }

        public static IQueryable<Invoice> FilterByDate(this IQueryable<Invoice> query, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue)
                query = query.Where(i => i.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.CreatedAt < toDate.Value.AddDays(1));

            return query;
        }

        public static IQueryable<Invoice> FilterByCustomer(this IQueryable<Invoice> query, long? customerId)
        {
            return customerId.HasValue
                ? query.Where(i => i.CustomerID == customerId.Value)
                : query;
        }

        public static IQueryable<Invoice> FilterByUser(this IQueryable<Invoice> query, long? userId)
        {
            return userId.HasValue
                ? query.Where(i => i.UserID == userId.Value)
                : query;
        }

        public static IQueryable<Invoice> FilterByAmount(
            this IQueryable<Invoice> query, decimal? minAmount, decimal? maxAmount)
        {
            if (minAmount.HasValue)
                query = query.Where(i => i.TotalAmount >= minAmount.Value);
            if (maxAmount.HasValue)
                query = query.Where(i => i.TotalAmount <= maxAmount.Value);

            return query;
        }

        public static IQueryable<Invoice> ApplySort(this IQueryable<Invoice> query, string? sortBy, bool isDescending)
        {
            var column = string.IsNullOrWhiteSpace(sortBy) ? "createdat" : sortBy.Trim().ToLower();

            if (!SortableColumns.Contains(column))
                column = "createdat";

            return column switch
            {
                "totalamount" => isDescending
                    ? query.OrderByDescending(i => i.TotalAmount)
                    : query.OrderBy(i => i.TotalAmount),
                "paidamount" => isDescending
                    ? query.OrderByDescending(i => i.PaidAmount)
                    : query.OrderBy(i => i.PaidAmount),
                _ => isDescending
                    ? query.OrderByDescending(i => i.CreatedAt)
                    : query.OrderBy(i => i.CreatedAt),
            };
        }
    }
}
