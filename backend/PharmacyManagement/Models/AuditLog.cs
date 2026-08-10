namespace PharmacyManagement.Models
{
	public class AuditLog
	{
		public long AuditLogID { get; set; }
        public ReferenceType TableName { get; set; }
        public TransactionType Action { get; set; }
        public String? OldValue { get; set; }
        public String? NewValue { get; set; }
        public long UserID { get; set; }
        public DateTime Date { get; set; }

        public User? User { get; set; }
    }
}
