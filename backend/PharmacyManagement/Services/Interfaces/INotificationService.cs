using PharmacyManagement.DTOs.Reconciliation;
using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendDebtNotificationAsync(CustomerDebtSummary debt);
        Task SendInvoiceCreatedAsync(string customerName, string customerEmail, long invoiceId, DateTime createdAt, decimal totalAmount);
        Task SendReceiptCreatedAsync(string customerName, string customerEmail, long receiptId, DateTime createdDate, decimal totalAmount);

        // Gửi cảnh báo khi job đối soát phát hiện hóa đơn bị lệch số tiền
        Task SendReconciliationAlertAsync(IReadOnlyList<ReconciliationIssue> issues, DateTime ranAt);
    }
}