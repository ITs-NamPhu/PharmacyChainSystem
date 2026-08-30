using PharmacyManagement.DTOs.Reconciliation;
using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace PharmacyManagement.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env; // lấy đường dẫn thực tế của project
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        public NotificationService(ILogger<NotificationService> logger, IEmailService emailService, IWebHostEnvironment env, IMemoryCache cache, IConfiguration config)
        {
            _logger = logger;
            _emailService = emailService;
            _env = env;
            _cache = cache;
            _config = config;
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

        public async Task SendInvoiceCreatedAsync(string customerName, string customerEmail, long invoiceId, DateTime createdAt, decimal totalAmount)
        {
            string cacheKey = "InvoiceEmailTemplate";

            // Đọc toàn bộ nội dung HTML lên (có cache 24h)
            string htmlTemplate = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // Thiết lập thời gian sống của Cache (Giữ trong 24 tiếng nếu không sử dụng)
                entry.SlidingExpiration = TimeSpan.FromHours(24);

                string templatePath = Path.Combine(_env.ContentRootPath, "Templates", "InvoiceCreatedEmail.html");
                return await File.ReadAllTextAsync(templatePath);
            }) ?? string.Empty;

            // Nối dữ liệu (Replace)
            // Hàm Replace tạo ra một chuỗi MỚI, không làm thay đổi nội dung đang lưu trong cache
            string finalHtml = htmlTemplate
                        .Replace("{{CustomerName}}", customerName)
                        .Replace("{{InvoiceNumber}}", invoiceId.ToString())
                        .Replace("{{CreatedDate}}", createdAt.ToString("dd/MM/yyyy HH:mm"))
                        .Replace("{{TotalAmount}}", totalAmount.ToString("N0"));

            string subject = $"[Nhà Thuốc IT] Hóa đơn điện tử {invoiceId} đã được tạo";
            await _emailService.SendEmailAsync(customerEmail, subject, finalHtml, isHtml: true);
        }

        public async Task SendReceiptCreatedAsync(string customerName, string customerEmail, long receiptId, DateTime createdDate, decimal totalAmount)
        {
            string cacheKey = "ReceiptEmailTemplate";

            // Đọc toàn bộ nội dung HTML lên (có cache 24h)
            string htmlTemplate = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // Thiết lập thời gian sống của Cache (Giữ trong 24 tiếng nếu không sử dụng)
                entry.SlidingExpiration = TimeSpan.FromHours(24);

                string templatePath = Path.Combine(_env.ContentRootPath, "Templates", "ReceiptCreatedEmail.html");
                return await File.ReadAllTextAsync(templatePath);
            }) ?? string.Empty;

            // Nối dữ liệu (Replace)
            // Hàm Replace tạo ra một chuỗi MỚI, không làm thay đổi nội dung đang lưu trong cache
            string finalHtml = htmlTemplate
                        .Replace("{{CustomerName}}", customerName)
                        .Replace("{{ReceiptID}}", receiptId.ToString())
                        .Replace("{{CreatedDate}}", createdDate.ToString("dd/MM/yyyy HH:mm"))
                        .Replace("{{TotalAmount}}", totalAmount.ToString("N0"));

            string subject = $"[Nhà Thuốc IT] Phiếu thu {receiptId} đã được tạo";
            await _emailService.SendEmailAsync(customerEmail, subject, finalHtml, isHtml: true);
        }

        public async Task SendReconciliationAlertAsync(IReadOnlyList<ReconciliationIssue> issues, DateTime ranAt)
        {
            // Đọc danh sách email nhận cảnh báo từ cấu hình appsettings.json
            var recipients = _config.GetSection("Reconciliation:AlertEmails").Get<List<string>>() ?? new();
            if (recipients.Count == 0)
            {
                _logger.LogWarning("Reconciliation alert not sent: no AlertEmails configured.");
                return;
            }

            // Dựng nội dung email: liệt kê từng hóa đơn bị lệch
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"<p>Job đối soát lúc <b>{ranAt:dd/MM/yyyy HH:mm}</b> phát hiện <b>{issues.Count}</b> hóa đơn bị lệch:</p>");
            sb.AppendLine("<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse'>");
            sb.AppendLine("<tr><th>InvoiceID</th><th>CustomerID</th><th>Total</th><th>PaidAmount</th><th>Applied</th><th>Trạng thái</th><th>Lý do</th></tr>");

            foreach (var issue in issues)
            {
                sb.AppendLine(
                    $"<tr><td>{issue.InvoiceID}</td><td>{issue.CustomerID}</td>" +
                    $"<td>{issue.TotalAmount:N0}</td><td>{issue.PaidAmount:N0}</td><td>{issue.AppliedAmount:N0}</td>" +
                    $"<td>{issue.PaymentStatus}</td><td>{issue.Reason}</td></tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("<p>Vui lòng kiểm tra và đối chiếu trực tiếp ngay.</p>");

            string subject = $"[CẢNH BÁO] Đối soát công nợ: {issues.Count} hóa đơn lệch";
            foreach (var recipient in recipients)
            {
                await _emailService.SendEmailAsync(recipient, subject, sb.ToString(), isHtml: true);
            }
        }
    }
}