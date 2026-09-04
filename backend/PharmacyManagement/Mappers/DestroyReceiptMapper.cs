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
                StockTakeID = request.StockTakeID,
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

        public static DestroyReceiptItem ToEntity(
            this UpdateDestroyReceiptItemRequest request,
            long destroyReceiptId,
            decimal unitCost)
        {
            return new DestroyReceiptItem
            {
                DestroyReceiptID = destroyReceiptId,
                BatchID = request.BatchID,
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
                StockTakeID = entity.StockTakeID,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status.ToString(),
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                IsFromStockTake = entity.StockTakeID.HasValue,
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
                StockTakeID = entity.StockTakeID,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status.ToString(),
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                IsFromStockTake = entity.StockTakeID.HasValue,
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
