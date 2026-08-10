using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PharmacyManagement.Models
{
	public class Unit
	{
		public long UnitID { get; set; }
        [StringLength(255)]
        public String UnitName { get; set; }

        public ICollection<Medicine>? Medicine { get; set; }
        public ICollection<UnitConversion>? UnitConversion { get; set; }
        public ICollection<InvoiceItem>? InvoiceItem { get; set; }
        public ICollection<GoodsReceiptItem>? GoodsReceiptItem { get; set; }

    }
}
