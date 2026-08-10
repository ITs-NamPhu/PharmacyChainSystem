using Microsoft.CodeAnalysis;
using System.Numerics;

namespace PharmacyManagement.Models
{

	public class CustomerDebtSummary
    {
		public long CustomerDebtSummaryID { get; set; }

        public DateTime Year { get; set; }
        public DateTime Month { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal Increase { get; set; }
        public decimal Paid { get; set; }
        public decimal ClosingBalance { get; set; }

        public Boolean IsLocked { get; set; }
        public long CustomerID { get; set; }

        public Customer? Customer { get; set; }
    }
}
