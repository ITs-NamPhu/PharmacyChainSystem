using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.Reconciliation;
using PharmacyManagement.Models;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Services.Implements
{
    public class ReconciliationService : IReconciliationService
    {
        private readonly PharmacySystemDbContext _context;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;

        public ReconciliationService(
            PharmacySystemDbContext context,
            IConfiguration config,
            INotificationService notificationService)
        {
            _context = context;
            _config = config;
            _notificationService = notificationService;
        }

        public async Task ReconcileAsync()
        {
            // Chỉ kiểm tra hóa đơn phát sinh từ ngày triển khai quy tắc mới
            var startDate = _config.GetValue<DateTime?>("Reconciliation:StartDate")
                            ?? new DateTime(2026, 1, 1);

            // Lấy toàn bộ hóa đơn (từ ngày triển khai) kèm tổng tiền đã gạch nợ thực tế
            var invoices = await _context.Invoice
                .Where(i => i.CreatedAt >= startDate)
                .Select(i => new
                {
                    i.InvoiceID,
                    i.CustomerID,
                    i.TotalAmount,
                    i.PaidAmount,
                    i.PaymentStatus,
                    AppliedAmount = i.ReceiptDetail!
                        .Where(d => !d.Receipt!.IsDeleted)
                        .Sum(d => (decimal?)d.AmountApplied) ?? 0m
                })
                .ToListAsync();

            var issues = new List<ReconciliationIssue>();

            foreach (var invoice in invoices)
            {
                // Trạng thái đúng sẽ là: Paid nếu đã trả hết, ngược lại là Debt
                var expectedStatus = invoice.PaidAmount >= invoice.TotalAmount
                    ? PaymentStatus.Paid
                    : PaymentStatus.Debt;

                var isAppliedMismatch = invoice.PaidAmount != invoice.AppliedAmount;
                var isStatusMismatch = invoice.PaymentStatus != expectedStatus;

                if (!isAppliedMismatch && !isStatusMismatch) continue;

                issues.Add(new ReconciliationIssue
                {
                    InvoiceID = invoice.InvoiceID,
                    CustomerID = invoice.CustomerID,
                    TotalAmount = invoice.TotalAmount,
                    PaidAmount = invoice.PaidAmount,
                    AppliedAmount = invoice.AppliedAmount,
                    PaymentStatus = invoice.PaymentStatus,
                    Reason = BuildReason(isAppliedMismatch, isStatusMismatch, invoice.AppliedAmount, invoice.PaidAmount)
                });
            }

            // Có lệch thì cảnh báo ngay cho team IT / Kế toán trưởng
            if (issues.Count > 0)
            {
                await _notificationService.SendReconciliationAlertAsync(issues, DateTime.Now);
            }
        }

        private static string BuildReason(
            bool isAppliedMismatch, bool isStatusMismatch,
            decimal appliedAmount, decimal paidAmount)
        {
            if (isAppliedMismatch)
                return $"PaidAmount ({paidAmount:N0}) khác tổng ReceiptDetail ({appliedAmount:N0})";

            if (isStatusMismatch)
                return "Trạng thái thanh toán không khớp với số tiền đã trả";

            return "Không xác định";
        }
    }
}