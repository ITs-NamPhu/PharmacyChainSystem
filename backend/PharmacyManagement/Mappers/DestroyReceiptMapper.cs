using PharmacyManagement.DTOs.DestroyReceipt;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class DestroyReceiptMapper
    {
        public static DestroyReceipt ToEntity(this CreateDestroyReceiptRequest request, long userId)
        {
            return new DestroyReceipt
            {
                WarehouseID = request.WarehouseID,
                UserID = userId,
                Note = request.Note ?? string.Empty,
                CreatedAt = DateTime.Now
            };
        }

        public static DestroyReceiptItem ToEntity(
            this CreateDestroyReceiptItemRequest request,
            long destroyReceiptId,
            decimal unitCost)
        {
            return new DestroyReceiptItem
            {
                DestroyReceiptID = destroyReceiptId,
                BatchID = request.BatchID,
                StockTakeItemID = request.StockTakeItemID,
                Quantity = request.Quantity,
                UnitCost = unitCost,
                ReasonCode = request.ReasonCode
            };
        }

        public static DestroyReceiptResponse ToResponse(this DestroyReceipt entity)
        {
            return new DestroyReceiptResponse
            {
                DestroyReceiptID = entity.DestroyReceiptID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                ItemCount = entity.DestroyReceiptItem?.Count ?? 0
            };
        }

        public static DestroyReceiptDetailResponse ToDetailResponse(this DestroyReceipt entity)
        {
            return new DestroyReceiptDetailResponse
            {
                DestroyReceiptID = entity.DestroyReceiptID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                Items = entity.DestroyReceiptItem?.Select(item => new DestroyReceiptItemDetailResponse
                {
                    DestroyReceiptItemID = item.DestroyReceiptItemID,
                    BatchID = item.BatchID,
                    StockTakeItemID = item.StockTakeItemID,
                    MedicineID = item.Batch?.GoodsReceiptItem?.MedicineID ?? 0,
                    MedicineName = item.Batch?.GoodsReceiptItem?.Medicine?.MedicineName ?? string.Empty,
                    UnitName = item.Batch?.GoodsReceiptItem?.UnitName ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    ReasonCode = item.ReasonCode
                }).ToList() ?? new List<DestroyReceiptItemDetailResponse>()
            };
        }

        public static List<DestroyReceiptResponse> ToResponseList(this List<DestroyReceipt> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
