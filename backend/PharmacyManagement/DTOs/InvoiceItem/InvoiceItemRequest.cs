namespace PharmacyManagement.DTOs.InvoiceItem
{
    public interface IInvoiceItemRequest
    {
        long MedicineID { get; set; }
        long? BatchID { get; set; }
        long UnitID { get; set; }
        decimal Quantity { get; set; }
        decimal UnitPrice { get; set; }

        public decimal? ConversionFactor { get; set; }
        public decimal? BaseQuantity { get; set; }
    }

    public class CreateInvoiceItemRequest : IInvoiceItemRequest
    {
        public long MedicineID { get; set; }
        public long? BatchID { get; set; }
        public long UnitID { get; set; }
        public decimal Quantity { get; set; }

        public decimal? ConversionFactor { get; set; }
        public decimal? BaseQuantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class UpdateInvoiceItemRequest : IInvoiceItemRequest
    {
        public long? InvoiceItemID { get; set; }
        public long MedicineID { get; set; }
        public long? BatchID { get; set; }
        public long UnitID { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal? ConversionFactor { get; set; }
        public decimal? BaseQuantity { get; set; }
    }
}
