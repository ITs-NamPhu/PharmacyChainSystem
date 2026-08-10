using PharmacyManagement.DTOs.InvoiceItem;
using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Implements
{
    public sealed record PreparedInvoiceItem(
        IInvoiceItemRequest Request,
        decimal BaseUnitQuantity,
        decimal ConversionRate,
        string UnitName,
        Batch? ManualBatch = null,
        IReadOnlyList<Batch>? AvailableBatches = null);
}
