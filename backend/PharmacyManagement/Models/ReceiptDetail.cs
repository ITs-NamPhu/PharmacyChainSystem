namespace PharmacyManagement.Models
{
    // Một dòng gạch nợ của phiếu thu.
    // Mỗi dòng nối 1 phiếu thu (Receipt) với 1 hóa đơn (Invoice) và ghi rõ số tiền áp dụng.
    public class ReceiptDetail
    {
        public long ReceiptDetailID { get; set; }
        public long ReceiptID { get; set; }
        public long InvoiceID { get; set; }

        // Số tiền của phiếu thu dùng để gạch cho hóa đơn này
        public decimal AmountApplied { get; set; }

        public Receipt? Receipt { get; set; }
        public Invoice? Invoice { get; set; }
    }
}