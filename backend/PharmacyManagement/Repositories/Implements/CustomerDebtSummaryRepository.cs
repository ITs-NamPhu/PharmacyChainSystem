using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class CustomerDebtSummaryRepository : ICustomerDebtSummaryRepository
    {
        private readonly PharmacySystemDbContext _context;

        public CustomerDebtSummaryRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsClosedAsync(int year, int month)
        {
            return await _context.CustomerDebtSummary
                .AnyAsync(x => x.Year == year && x.Month == month);
        }

        public async Task<Dictionary<long, decimal>> GetClosingBalancesAsync(int year, int month)
        {
            return (await _context.CustomerDebtSummary
                .Where(x => x.Year == year && x.Month == month && x.IsLocked)
                .Select(x => new { x.CustomerID, x.ClosingBalance })
                .ToListAsync())
                .GroupBy(x => x.CustomerID)
                .ToDictionary(g => g.Key, g => g.First().ClosingBalance);
        }

        public async Task<Dictionary<long, decimal>> GetInvoiceIncreasesAsync(DateTime startDate, DateTime endDate)
        {
            return (await _context.Invoice
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .Select(x => new { x.CustomerID, Amount = x.TotalAmount - x.PaidAmount })
                .ToListAsync())
                .GroupBy(x => x.CustomerID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
        }

        public async Task<Dictionary<long, decimal>> GetReceiptPaymentsAsync(DateTime startDate, DateTime endDate)
        {
            // Tổng tiền thực tế gạch nợ của khách trong kỳ.
            // Dùng SUM(ReceiptDetail.AmountApplied) thay vì SUM(Receipt.TotalAmount)
            // vì Receipt.TotalAmount có thể gồm cả phần nạp vào ví (tiền thừa).
            return (await _context.Receipt
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                .SelectMany(x => x.ReceiptDetail!)
                .Select(d => new { d.Invoice!.CustomerID, d.AmountApplied })
                .ToListAsync())
                .GroupBy(x => x.CustomerID)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.AmountApplied));
        }

        public async Task AddRangeAsync(IEnumerable<CustomerDebtSummary> summaries)
        {
            await _context.CustomerDebtSummary.AddRangeAsync(summaries);
        }

        public async Task<List<CustomerDebtSummary>> GetDebtorsAsync(int year, int month)
        {
            return await _context.CustomerDebtSummary
                .Include(x => x.Customer)
                .Where(x => x.Year == year && x.Month == month && x.ClosingBalance > 0)
                .ToListAsync();
        }

        public async Task<List<CustomerDebtSummary>> GetByMonthAsync(int year, int month, long? customerId, int skip, int take)
        {
            var query = _context.CustomerDebtSummary
                .Include(x => x.Customer)
                .Where(x => x.Year == year && x.Month == month);

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerID == customerId.Value);

            return await query
                .OrderByDescending(x => x.ClosingBalance)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountByMonthAsync(int year, int month, long? customerId)
        {
            var query = _context.CustomerDebtSummary
                .Where(x => x.Year == year && x.Month == month);

            if (customerId.HasValue)
                query = query.Where(x => x.CustomerID == customerId.Value);

            return await query.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
