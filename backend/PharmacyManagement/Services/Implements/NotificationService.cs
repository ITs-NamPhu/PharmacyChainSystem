using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IEmailService _emailService;

        public NotificationService(ILogger<NotificationService> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }


        public async Task SendDebtNotificationAsync(CustomerDebtSummary debt)
        {
            var subject = $"Thông báo dư nợ tháng {debt.Month}/{debt.Year}";

            var message = string.Format(
                "Kính gửi quý khách, tổng dư nợ của quý khách đến hết tháng {0}/{1} là: {2:N0} VNĐ. Chi tiết: Phát sinh {3:N0} - Đã trả {4:N0}.",
                debt.Month, debt.Year, debt.ClosingBalance, debt.Increase, debt.Paid);

            var customer = debt.Customer;

            if (customer != null && !string.IsNullOrWhiteSpace(customer.Email))
            {
                // gửi email: nối provider gửi email
                await _emailService.SendEmailAsync(customer.Email, subject, message);
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

        private Task SendSmsAsync(string phone, string message)
        {
            _logger.LogInformation(
                "[SMS] To: {Phone} | Body: {Message}",
                phone, message);
            return Task.CompletedTask;
        }
    }
}
