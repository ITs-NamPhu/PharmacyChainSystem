using PharmacyManagement.DTOs.GoodsReceipt;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class GoodsReceiptMapper
    {
        public static GoodsReceipt ToEntity(this CreateGoodsReceiptRequest request, long userId, long branchId)
        {
            return new GoodsReceipt
            {
                SupplierID = request.SupplierID,
                BranchID = branchId,
                UserID = userId,
                ReceiptDate = request.ReceiptDate,
                TotalAmount = 0,
                PaidAmount = request.PaidAmount,
                Note = request.Note ?? string.Empty
            };
        }

        public static void ApplyTo(this UpdateGoodsReceiptRequest request, GoodsReceipt entity)
        {
            entity.SupplierID = request.SupplierID;
            entity.ReceiptDate = request.ReceiptDate;
            entity.PaidAmount = request.PaidAmount;
            entity.Note = request.Note ?? string.Empty;
        }

        public static GoodsReceiptItem ToEntity(
            this CreateGoodsReceiptItemRequest request,
            long goodsReceiptId,
            decimal conversionFactor,
            string unitName)
        {
            return new GoodsReceiptItem
            {
                GoodsReceiptID = goodsReceiptId,
                MedicineID = request.MedicineID,
                UnitID = request.UnitID,
                UnitName = unitName,
                Quantity = request.Quantity,
                ConversionFactor = conversionFactor,
                UnitCost = request.UnitCost
            };
        }

        public static GoodsReceiptItem ToEntity(
            this UpdateGoodsReceiptItemRequest request,
            long goodsReceiptId,
            decimal conversionFactor,
            string unitName)
        {
            return new GoodsReceiptItem
            {
                GoodsReceiptItemID = request.GoodsReceiptItemID ?? 0,
                GoodsReceiptID = goodsReceiptId,
                MedicineID = request.MedicineID,
                UnitID = request.UnitID,
                UnitName = unitName,
                Quantity = request.Quantity,
                ConversionFactor = conversionFactor,
                UnitCost = request.UnitCost
            };
        }

        public static void ApplyTo(
            this UpdateGoodsReceiptItemRequest request,
            GoodsReceiptItem entity,
            decimal conversionFactor,
            string unitName)
        {
            entity.UnitID = request.UnitID;
            entity.UnitName = unitName;
            entity.Quantity = request.Quantity;
            entity.ConversionFactor = conversionFactor;
            entity.UnitCost = request.UnitCost;
        }

        public static GoodsReceiptResponse ToResponse(this GoodsReceipt entity)
        {
            return new GoodsReceiptResponse
            {
                GoodsReceiptID = entity.GoodsReceiptID,
                ReceiptNumber = entity.ReceiptNumber,
                SupplierName = entity.Supplier?.SupplierName ?? string.Empty,
                UserName = entity.User?.FullName ?? string.Empty,
                ReceiptDate = entity.ReceiptDate,
                TotalAmount = entity.TotalAmount,
                PaidAmount = entity.PaidAmount,
                Note = entity.Note
            };
        }

        public static GoodsReceiptDetailResponse ToDetailResponse(this GoodsReceipt entity)
        {
            return new GoodsReceiptDetailResponse
            {
                GoodsReceiptID = entity.GoodsReceiptID,
                ReceiptNumber = entity.ReceiptNumber,
                SupplierID = entity.SupplierID,
                SupplierName = entity.Supplier?.SupplierName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                ReceiptDate = entity.ReceiptDate,
                TotalAmount = entity.TotalAmount,
                PaidAmount = entity.PaidAmount,
                Note = entity.Note,
                Items = entity.GoodsReceiptItem?.Select(item => new GoodsReceiptItemDetailResponse
                {
                    GoodsReceiptItemID = item.GoodsReceiptItemID,
                    MedicineID = item.MedicineID,
                    MedicineName = item.Medicine?.MedicineName ?? string.Empty,
                    UnitID = item.UnitID,
                    UnitName = item.UnitName,
                    Quantity = item.Quantity,
                    ConversionFactor = item.ConversionFactor,
                    UnitCost = item.UnitCost,
                    QuantityReceived = item.Batch?.Sum(b => b.QuantityReceived) ?? 0,
                    QuantityInStock = item.Batch?.Sum(b => b.QuantityInStock) ?? 0,
                    ManufactureDate = item.Batch?.FirstOrDefault()?.ManufactureDate ?? DateTime.MinValue,
                    ExpiryDate = item.Batch?.FirstOrDefault()?.ExpiryDate ?? DateTime.MaxValue
                }).ToList() ?? new List<GoodsReceiptItemDetailResponse>()
            };
        }

        public static List<GoodsReceiptResponse> ToResponseList(this List<GoodsReceipt> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
