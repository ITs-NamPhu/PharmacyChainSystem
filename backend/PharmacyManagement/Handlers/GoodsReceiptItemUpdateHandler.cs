using PharmacyManagement.DTOs.GoodsReceipt;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Handlers
{
    public class GoodsReceiptItemUpdateHandler
    {
        public void ValidateItemUpdate(
            UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem existingItem,
            BatchExportStatus status,
            decimal conversionFactor)
        {
            if (status.HasBeenExported)
            {
                ValidateSoldItem(request, existingItem, status, conversionFactor);
            }
        }

        public void ValidateItemDeletion(
            GoodsReceiptItem existingItem,
            BatchExportStatus status)
        {
            if (status.HasBeenExported)
            {
                throw new BusinessException(
                    $"Cannot delete item '{existingItem.Medicine?.MedicineName}' " +
                    $"because it has been sold ({status.QuantityExported} units).",
                    "GR004",
                    StatusCodes.Status400BadRequest);
            }
        }

        public void ApplyUpdate(
            UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem existingItem,
            BatchExportStatus status,
            decimal conversionFactor,
            string unitName)
        {
            if (!status.HasBeenExported)
            {
                ApplyUnsoldItemUpdate(request, existingItem, conversionFactor, unitName);
            }
            else
            {
                ApplySoldItemUpdate(request, existingItem, status, conversionFactor, unitName);
            }
        }

        private void ValidateSoldItem(
            UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem existingItem,
            BatchExportStatus status,
            decimal conversionFactor)
        {
            if (existingItem.MedicineID != request.MedicineID)
            {
                throw new BusinessException(
                    $"Cannot change medicine for item '{existingItem.Medicine?.MedicineName}' " +
                    $"because it has been sold.",
                    "GR005",
                    StatusCodes.Status400BadRequest);
            }

            if (conversionFactor < status.QuantityExported)
            {
                throw new BusinessException(
                    $"New quantity ({request.Quantity}) must be at least " +
                    $"{status.QuantityExported} (already sold) " +
                    $"for item '{existingItem.Medicine?.MedicineName}'.",
                    "GR006",
                    StatusCodes.Status400BadRequest);
            }
        }

        private void ApplyUnsoldItemUpdate(
            UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem existingItem,
            decimal conversionFactor,
            string unitName)
        {
            request.ApplyTo(existingItem, conversionFactor, unitName);

            var batch = existingItem.Batch?.FirstOrDefault();
            if (batch != null)
            {
                batch.ManufactureDate = request.ManufactureDate;
                batch.ExpiryDate = request.ExpiryDate;
                batch.QuantityReceived = conversionFactor * request.Quantity;
                batch.QuantityInStock = batch.QuantityReceived;
            }
        }

        private void ApplySoldItemUpdate(
            UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem existingItem,
            BatchExportStatus status,
            decimal conversionFactor,
            string unitName)
        {
            request.ApplyTo(existingItem, conversionFactor, unitName);

            var batch = existingItem.Batch?.FirstOrDefault();
            if (batch != null)
            {
                batch.QuantityReceived = conversionFactor * request.Quantity;
                batch.QuantityInStock = batch.QuantityReceived - status.QuantityExported;
                batch.ExpiryDate = request.ExpiryDate;
                batch.ManufactureDate = request.ManufactureDate;
            }
        }
    }
}
