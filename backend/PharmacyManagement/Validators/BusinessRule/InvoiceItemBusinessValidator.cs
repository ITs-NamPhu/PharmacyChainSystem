using PharmacyManagement.DTOs.InvoiceItem;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class InvoiceItemBusinessValidator
    {
        public void ValidateFefoItemAsync(IInvoiceItemRequest item, decimal baseUnitQuantity, decimal available)
        {
            if (available < baseUnitQuantity)
            {
                throw new BusinessException(
                    $"Insufficient stock for medicine (ID: {item.MedicineID}). " +
                    $"Available: {available}, Requested: {baseUnitQuantity}.",
                    "INV009",
                    StatusCodes.Status400BadRequest);
            }
        }

        public Batch ValidateManualItemAsync(IInvoiceItemRequest item, decimal baseUnitQuantity, Batch batch)
        {
            if (!item.BatchID.HasValue)
            {
                throw new BusinessException(
                    "BatchID is required for manual mode.",
                    "INV010",
                    StatusCodes.Status400BadRequest);
            }

            if (batch == null)
            {
                throw new BusinessException(
                    "Batch not found.",
                    "INV002",
                    StatusCodes.Status404NotFound);
            }

            if (batch.GoodsReceiptItem?.MedicineID != item.MedicineID)
            {
                throw new BusinessException(
                    "Batch does not belong to the selected medicine.",
                    "INV003",
                    StatusCodes.Status400BadRequest);
            }

            if (batch.QuantityInStock < baseUnitQuantity)
            {
                throw new BusinessException(
                    $"Insufficient stock. Available: {batch.QuantityInStock}, Requested: {baseUnitQuantity}.",
                    "INV004",
                    StatusCodes.Status400BadRequest);
            }

            return batch;
        }
    }
}
