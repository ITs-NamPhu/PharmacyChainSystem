using PharmacyManagement.DTOs.Invoice;
using PharmacyManagement.DTOs.InvoiceItem;
using PharmacyManagement.DTOs.Batch;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class InvoiceMapper
    {
        public static Models.Invoice ToEntity(this CreateInvoiceRequest request, long userId, long branchId)
        {
            return new Models.Invoice
            {
                CustomerID = request.CustomerID,
                BranchID = branchId,
                UserID = userId,
                CreatedAt = DateTime.Now,
                TotalAmount = 0,
                PaidAmount = 0,
                Note = request.Note ?? string.Empty
            };
        }

        public static void ApplyTo(this UpdateInvoiceRequest request, Models.Invoice entity)
        {
            entity.CustomerID = request.CustomerID;
            entity.Note = request.Note ?? string.Empty;
            if (request.CreatedByUserID.HasValue)
                entity.UserID = request.CreatedByUserID.Value;
        }

        public static Models.InvoiceItem ToEntity(
            this IInvoiceItemRequest request,
            long invoiceId,
            Batch batch,
            decimal conversionFactor,
            decimal baseQuantity,
            string unitName)
        {
            return new Models.InvoiceItem
            {
                InvoiceID = invoiceId,
                BatchID = batch.BatchID,
                UnitID = request.UnitID,
                UnitName = unitName,
                Quantity = request.Quantity,
                ConversionFactor = conversionFactor,
                BaseQuantity = baseQuantity,
                UnitPrice = request.UnitPrice
            };
        }

        public static InvoiceResponse ToResponse(this Models.Invoice entity)
        {
            return new InvoiceResponse
            {
                InvoiceID = entity.InvoiceID,
                CustomerName = entity.Customer?.CustomerName ?? string.Empty,
                TotalAmount = entity.TotalAmount,
                PaidAmount = entity.PaidAmount,
                CreatedAt = entity.CreatedAt,
                UserName = entity.User?.FullName ?? string.Empty
            };
        }

        public static InvoiceDetailResponse ToDetailResponse(this Models.Invoice entity)
        {
            return new InvoiceDetailResponse
            {
                InvoiceID = entity.InvoiceID,
                CustomerID = entity.CustomerID,
                CustomerName = entity.Customer?.CustomerName ?? string.Empty,
                TotalAmount = entity.TotalAmount,
                PaidAmount = entity.PaidAmount,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                UserName = entity.User?.FullName ?? string.Empty,
                UserID = entity.UserID,
                InvoiceItems = entity.InvoiceItem?.Select(ii => new InvoiceItemDetailResponse
                {
                    InvoiceItemID = ii.InvoiceItemID,
                    MedicineID = ii.Batch?.GoodsReceiptItem?.MedicineID ?? 0,
                    MedicineName = ii.Batch?.GoodsReceiptItem?.Medicine?.MedicineName ?? string.Empty,
                    BatchID = ii.BatchID,
                    UnitID = ii.UnitID,
                    UnitName = ii.UnitName,
                    Quantity = ii.Quantity,
                    UnitPrice = ii.UnitPrice,
                    SubTotal = ii.Quantity * ii.UnitPrice
                }).ToList() ?? new List<InvoiceItemDetailResponse>()
            };
        }

        public static List<InvoiceResponse> ToResponseList(this List<Models.Invoice> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public static BatchByMedicineResponse ToBatchResponse(this Batch entity)
        {
            return new BatchByMedicineResponse
            {
                BatchID = entity.BatchID,
                QuantityInStock = entity.QuantityInStock,
                ExpiryDate = entity.ExpiryDate,
                UnitCost = entity.GoodsReceiptItem?.UnitCost ?? 0,
            };
        }

        public static AllocatedBatchDto ToAllocatedDto(this Batch entity, decimal quantityAllocated)
        {
            return new AllocatedBatchDto
            {
                BatchID = entity.BatchID,
                QuantityAllocated = quantityAllocated,
                ExpiryDate = entity.ExpiryDate,
                QuantityInStock = entity.QuantityInStock
            };
        }
    }
}
