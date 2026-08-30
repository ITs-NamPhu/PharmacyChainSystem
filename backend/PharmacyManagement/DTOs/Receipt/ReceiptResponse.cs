using PharmacyManagement.Models;

namespace PharmacyManagement.DTOs.Receipt
{
    public class CreateReceiptRequest
    {
        public long CustomerID { get; set; }

        // Số tiền khách thực đưa (VD: 1.000.000đ).
        // Backend sẽ tự tìm hóa đơn nợ cũ nhất và gạch theo FIFO.
        public decimal TotalAmount { get; set; }
    }

    public class UpdateReceiptRequest
    {
        public long CustomerID { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ReceiptResponse
    {
        public long ReceiptID { get; set; }
        public long CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public long BranchID { get; set; }
        public string? BranchName { get; set; }
        public long UserID { get; set; }
        public string? UserName { get; set; }

        // Tổng số tiền khách thực đưa
        public decimal TotalAmount { get; set; }

        // Phương thức thanh toán: CASH hoặc WALLET
        public PaymentMethod PaymentMethod { get; set; }

        // Tổng số tiền đã dùng để gạch nợ cho các hóa đơn
        public decimal AmountApplied { get; set; }

        // Số tiền dư (nếu có) đã nạp vào ví khách hàng
        public decimal WalletCredit { get; set; }

        // Tổng nợ còn lại của khách sau phiếu thu này
        public decimal RemainingDebt { get; set; }

        public DateTime CreatedDate { get; set; }

        // Chi tiết gạch nợ theo FIFO
        public List<ReceiptDetailResponse> Details { get; set; } = new();
    }

    public class ReceiptDetailResponse
    {
        public long ReceiptDetailID { get; set; }
        public long InvoiceID { get; set; }

        // Số tiền của phiếu thu áp dụng cho hóa đơn này
        public decimal AmountApplied { get; set; }

        // Thông tin hóa đơn để hiển thị
        public DateTime InvoiceCreatedAt { get; set; }
        public decimal InvoiceTotalAmount { get; set; }
        public PaymentStatus InvoicePaymentStatus { get; set; }
    }

    public class ReceiptListResponse
    {
        public int NumRecords { get; set; }
        public int TotalPage { get; set; }
        public List<ReceiptResponse> Items { get; set; } = new();
    }
}