namespace PharmacyManagement.Models
{
    // Trạng thái thanh toán của hóa đơn: còn nợ hoặc đã trả hết
    public enum PaymentStatus
    {
        Debt = 0,   // Còn nợ
        Paid = 1    // Đã trả hết
    }

    // Phương thức thanh toán của phiếu thu
    public enum PaymentMethod
    {
        CASH = 0,   // Tiền mặt
        WALLET = 1  // Trả bằng số dư ví
    }

    // Loại giao dịch của sổ phụ ví khách hàng
    public enum WalletTransactionType
    {
        IN = 0,     // Tiền vào ví (nạp tiền / tiền thừa)
        OUT = 1     // Tiền ra khỏi ví (dùng ví để trả)
    }

    // Nguồn gốc của giao dịch ví
    public enum WalletRefType
    {
        RECEIPT = 0,    // Từ phiếu thu (tiền thừa nạp vào ví)
        INVOICE = 1     // Từ hóa đơn (dùng ví để trả)
    }
}