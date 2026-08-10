namespace PharmacyManagement.DTOs.InvoiceItem
{
    public class InvoiceItemDetailResponse
    {
        public long InvoiceItemID { get; set; }
        public long MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public long BatchID { get; set; }
        public long UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
