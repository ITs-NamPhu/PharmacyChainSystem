using PharmacyManagement.share;

namespace PharmacyManagement.Models
{
	public class Receipt : ISoftDelete
	{
		public long ReceiptID { get; set; }
        public long CustomerID { get; set; }

        // Chi nhánh tiếp nhận phiếu thu (nơi khách thực đưa tiền)
        public long BranchID { get; set; }

        public long UserID { get; set; }

        // Tổng số tiền khách thực đưa (gồm cả phần nạp vào ví nếu có dư)
        public decimal TotalAmount { get; set; }

        // Phương thức thanh toán: CASH (tiền mặt) hoặc WALLET (trả bằng ví)
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime CreatedDate { get; set; }

        public Customer? Customer { get; set; }
        public Branch? Branch { get; set; }
        public User? User { get; set; }

        public ICollection<ReceiptDetail>? ReceiptDetail { get; set; }

        public bool IsDeleted { get; set; }
    }
}