using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagement.Models
{
	public class InventoryTransaction
	{
		public long InventoryTransactionID { get; set; }
        public long BatchID { get; set; }
        // chứng từ số
        public long ReferenceID { get; set; }
        public long ActionBy { get; set; }

        public TransactionType TransactionType { get; set; }
        public ReferenceType ReferenceType { get; set; }
        public decimal QuantityChange { get; set; }
        public DateTime CreatedAt { get; set; }

        public Batch? Batch { get; set; }
        public User? User { get; set; }
	}

    public enum TransactionType
    {
        Create,
        Update,
        Delete
    }
    public enum ReferenceType
    {
        Invoice,
        Import,
        PurchaseReturn,
        SaleReturn
    }
}
