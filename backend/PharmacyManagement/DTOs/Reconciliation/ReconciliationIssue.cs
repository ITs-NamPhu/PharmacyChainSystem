using PharmacyManagement.Models;

namespace PharmacyManagement.DTOs.Reconciliation
{
    // Một hóa đơn bị phát hiện lệch khi đối soát
    public class ReconciliationIssue
    {
        public long InvoiceID { get; set; }
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }

        // Số tiền đang ghi trên hóa đơn
        public decimal PaidAmount { get; set; }

        // Số tiền thực tế đã áp dụng (SUM ReceiptDetail)
        public decimal AppliedAmount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        // Mô tả ngắn lệch chỗ nào (giúp kế toán kiểm tra nhanh)
        public string Reason { get; set; } = string.Empty;
    }
}