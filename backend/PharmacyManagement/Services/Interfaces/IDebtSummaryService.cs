using PharmacyManagement.DTOs.CustomerDebtSummary;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IDebtSummaryService
    {
        Task ProcessMonthlyClosingAsync(int targetYear, int targetMonth);
        Task<CustomerDebtSummaryListResponse> GetByMonthAsync(int year, int month, long? customerId, int page, int count);
    }
}
