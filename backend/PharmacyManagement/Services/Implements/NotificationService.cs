using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendDebtNotificationAsync(CustomerDebtSummary debt)
        {
            var message = string.Format(
                "Kính gửi quý khách, tổng dư nợ của quý khách đến hết tháng {0}/{1} là: {2:N0} VNĐ. Chi tiết: Phát sinh {3:N0} - Đã trả {4:N0}.",
                debt.Month, debt.Year, debt.ClosingBalance, debt.Increase, debt.Paid);

            var customer = debt.Customer;

            if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
            {
                // Kênh email: nối provider gửi email thật tại đây (hiện tại ghi log placeholder)
                await SendEmailAsync(customer.Email, message);
            }
            else if (customer != null && !string.IsNullOrWhiteSpace(customer.Phone))
            {
                // Kênh SMS: nối provider gửi SMS/Zalo thật tại đây (hiện tại ghi log placeholder)
                await SendSmsAsync(customer.Phone, message);
            }
            else
            {
                _logger.LogWarning(
                    "Debt notification not sent for customer {CustomerId}: no email/phone on file. {Message}",
                    debt.CustomerID, message);
            }
        }

        private Task SendEmailAsync(string email, string message)
        {
            _logger.LogInformation(
                "[EMAIL] To: {Email} | Subject: Thông báo dư nợ | Body: {Message}",
                email, message);
            return Task.CompletedTask;
        }

        private Task SendSmsAsync(string phone, string message)
        {
            _logger.LogInformation(
                "[SMS] To: {Phone} | Body: {Message}",
                phone, message);
            return Task.CompletedTask;
        }
    }
}
