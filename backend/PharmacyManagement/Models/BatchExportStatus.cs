namespace PharmacyManagement.Models
{
    public record BatchExportStatus
    {
        public decimal QuantityReceived { get; init; }
        public decimal QuantityInStock { get; init; }
        public decimal QuantityExported => QuantityReceived - QuantityInStock;
        public bool HasBeenExported => QuantityExported > 0;

        public static BatchExportStatus From(Batch batch) => new()
        {
            QuantityReceived = batch.QuantityReceived,
            QuantityInStock = batch.QuantityInStock
        };

        public static BatchExportStatus NoBatch() => new()
        {
            QuantityReceived = 0,
            QuantityInStock = 0
        };
    }
}
