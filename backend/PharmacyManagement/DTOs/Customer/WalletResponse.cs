using PharmacyManagement.Models;

namespace PharmacyManagement.DTOs.Customer
{
    // Trả về số dư ví và lịch sử biến động của khách hàng
    public class CustomerWalletResponse
    {
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        // Số dư ví hiện tại
        public decimal WalletBalance { get; set; }

        public List<CustomerWalletHistoryResponse> History { get; set; } = new();
    }

    public class CustomerWalletHistoryResponse
    {
        public long CustomerWalletHistoryID { get; set; }

        // IN (tiền vào ví) hoặc OUT (tiền ra khỏi ví)
        public WalletTransactionType TransactionType { get; set; }

        public decimal Amount { get; set; }

        // Nguồn gốc: RECEIPT (phiếu thu) hoặc INVOICE (hóa đơn)
        public WalletRefType RefType { get; set; }
        public long RefId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}