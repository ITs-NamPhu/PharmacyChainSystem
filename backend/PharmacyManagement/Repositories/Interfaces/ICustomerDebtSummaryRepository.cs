using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface ICustomerDebtSummaryRepository
    {
        Task<bool> IsClosedAsync(int year, int month);
        Task<Dictionary<long, decimal>> GetClosingBalancesAsync(int year, int month);
        Task<Dictionary<long, decimal>> GetInvoiceIncreasesAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<long, decimal>> GetReceiptPaymentsAsync(DateTime startDate, DateTime endDate);
        Task AddRangeAsync(IEnumerable<CustomerDebtSummary> summaries);
        Task<List<CustomerDebtSummary>> GetDebtorsAsync(int year, int month);
        Task<List<CustomerDebtSummary>> GetByMonthAsync(int year, int month, long? customerId, int skip, int take);
        Task<int> CountByMonthAsync(int year, int month, long? customerId);
        Task SaveChangesAsync();
    }
}
