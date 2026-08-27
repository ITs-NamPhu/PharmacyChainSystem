using PharmacyManagement.DTOs.CustomerDebtSummary;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;

namespace PharmacyManagement.Services.Implements
{
    public class DebtSummaryService : IDebtSummaryService
    {
        private readonly ICustomerDebtSummaryRepository _repository;
        private readonly INotificationService _notificationService;

        public DebtSummaryService(
            ICustomerDebtSummaryRepository repository,
            INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task ProcessMonthlyClosingAsync(int targetYear, int targetMonth)
        {
            // Idempotency: đã chốt sổ tháng này rồi thì dừng lại
            var isAlreadyClosed = await _repository.IsClosedAsync(targetYear, targetMonth);
            if (isAlreadyClosed) return;

            var startDate = new DateTime(targetYear, targetMonth, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            // Xác định tháng trước để lấy Dư đầu kỳ (Opening Balance)
            var prevMonthDate = startDate.AddMonths(-1);
            var prevYear = prevMonthDate.Year;
            var prevMonth = prevMonthDate.Month;

            // 1. Dư cuối kỳ của tháng trước (đã khóa) -> Dư đầu kỳ
            var prevBalances = await _repository.GetClosingBalancesAsync(prevYear, prevMonth);

            // 2. Tổng nợ tăng trong kỳ từ Invoice
            var increases = await _repository.GetInvoiceIncreasesAsync(startDate, endDate);

            // 3. Tổng đã thanh toán trong kỳ từ Receipt
            var payments = await _repository.GetReceiptPaymentsAsync(startDate, endDate);

            // 4. Danh sách khách hàng có phát sinh giao dịch hoặc có dư nợ cũ
            var activeCustomerIds = prevBalances.Keys
                .Union(increases.Keys)
                .Union(payments.Keys)
                .ToList();

            var newSummaries = new List<CustomerDebtSummary>();

            // 5. Tính toán chốt sổ
            foreach (var customerId in activeCustomerIds)
            {
                decimal opening = prevBalances.GetValueOrDefault(customerId, 0m);
                decimal increase = increases.GetValueOrDefault(customerId, 0m);
                decimal paid = payments.GetValueOrDefault(customerId, 0m);
                decimal closing = opening + increase - paid;

                // Bỏ qua nếu khách không có nợ cũ và cũng không phát sinh gì
                if (opening == 0 && increase == 0 && paid == 0) continue;

                newSummaries.Add(new CustomerDebtSummary
                {
                    CustomerID = customerId,
                    Year = targetYear,
                    Month = targetMonth,
                    OpeningBalance = opening,
                    Increase = increase,
                    Paid = paid,
                    ClosingBalance = closing,
                    IsLocked = true
                });
            }

            // 6. Lưu vào Database
            await _repository.AddRangeAsync(newSummaries);
            await _repository.SaveChangesAsync();

            // 7. Gửi thông báo cho khách hàng có dư nợ > 0
            await NotifyCustomersAsync(targetYear, targetMonth);
        }

        private async Task NotifyCustomersAsync(int year, int month)
        {
            var debtors = await _repository.GetDebtorsAsync(year, month);

            foreach (var debt in debtors)
            {
                await _notificationService.SendDebtNotificationAsync(debt);
            }
        }

        public async Task<CustomerDebtSummaryListResponse> GetByMonthAsync(int year, int month, long? customerId, int page, int count)
        {
            var skip = PaginationHelper.GetSkip(page, count);

            var items = await _repository.GetByMonthAsync(year, month, customerId, skip, count);
            var numRecords = await _repository.CountByMonthAsync(year, month, customerId);

            return new CustomerDebtSummaryListResponse
            {
                NumRecords = numRecords,
                TotalPage = (int)PaginationHelper.GetTotalPage(numRecords, count),
                Items = items.ToResponseList()
            };
        }
    }
}
