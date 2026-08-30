namespace PharmacyManagement.Models
{
    // Sổ phụ ví khách hàng: ghi lại từng lần tiền vào/ra khỏi ví
    public class CustomerWalletHistory
    {
        public long CustomerWalletHistoryID { get; set; }
        public long CustomerID { get; set; }

        // IN (tiền vào ví) hoặc OUT (tiền ra khỏi ví)
        public WalletTransactionType TransactionType { get; set; }

        // Số tiền biến động
        public decimal Amount { get; set; }

        // Nguồn gốc giao dịch: RECEIPT (phiếu thu) hoặc INVOICE (hóa đơn)
        public WalletRefType RefType { get; set; }

        // ID của phiếu thu hoặc hóa đơn liên quan
        public long RefId { get; set; }

        public DateTime CreateDate { get; set; }

        public Customer? Customer { get; set; }
    }
}