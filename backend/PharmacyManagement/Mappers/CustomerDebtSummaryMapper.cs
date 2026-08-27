using PharmacyManagement.DTOs.CustomerDebtSummary;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class CustomerDebtSummaryMapper
    {
        public static CustomerDebtSummaryResponse ToResponse(this CustomerDebtSummary entity)
        {
            return new CustomerDebtSummaryResponse
            {
                CustomerDebtSummaryID = entity.CustomerDebtSummaryID,
                Year = entity.Year,
                Month = entity.Month,
                OpeningBalance = entity.OpeningBalance,
                Increase = entity.Increase,
                Paid = entity.Paid,
                ClosingBalance = entity.ClosingBalance,
                IsLocked = entity.IsLocked,
                CustomerID = entity.CustomerID,
                CustomerName = entity.Customer?.CustomerName ?? string.Empty,
                Phone = entity.Customer?.Phone ?? string.Empty,
                Email = entity.Customer?.Email ?? string.Empty
            };
        }

        public static List<CustomerDebtSummaryResponse> ToResponseList(this IEnumerable<CustomerDebtSummary> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
