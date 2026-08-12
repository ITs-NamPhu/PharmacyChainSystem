using PharmacyManagement.DTOs.StockAdjustment;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class StockAdjustmentMapper
    {
        public static StockAdjustment ToEntity(this CreateStockAdjustmentRequest request, long userId)
        {
            return new StockAdjustment
            {
                WarehouseID = 0,
                UserID = userId,
                StockTakeID = request.StockTakeID,
                Note = request.Note,
                CreatedAt = DateTime.Now
            };
        }

        public static StockAdjustmentItem ToEntity(
            this CreateStockAdjustmentItemRequest request,
            long stockAdjustmentId)
        {
            return new StockAdjustmentItem
            {
                StockAdjustmentID = stockAdjustmentId,
                BatchID = request.BatchID,
                StockTakeItemID = request.StockTakeItemID,
                AdjustQuantity = request.AdjustQuantity,
                ReasonCode = request.ReasonCode
            };
        }

        public static StockAdjustmentResponse ToResponse(this StockAdjustment entity)
        {
            return new StockAdjustmentResponse
            {
                StockAdjustmentID = entity.StockAdjustmentID,
                StockTakeID = entity.StockTakeID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                Note = entity.Note ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                ItemCount = entity.StockAdjustmentItem?.Count ?? 0
            };
        }

        public static StockAdjustmentDetailResponse ToDetailResponse(this StockAdjustment entity)
        {
            return new StockAdjustmentDetailResponse
            {
                StockAdjustmentID = entity.StockAdjustmentID,
                StockTakeID = entity.StockTakeID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                Note = entity.Note ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                Items = entity.StockAdjustmentItem?.Select(item => new StockAdjustmentItemDetailResponse
                {
                    StockAdjustmentItemID = item.StockAdjustmentItemID,
                    BatchID = item.BatchID,
                    StockTakeItemID = item.StockTakeItemID,
                    MedicineID = item.Batch?.GoodsReceiptItem?.MedicineID ?? 0,
                    MedicineName = item.Batch?.GoodsReceiptItem?.Medicine?.MedicineName ?? string.Empty,
                    UnitName = item.Batch?.GoodsReceiptItem?.UnitName ?? string.Empty,
                    AdjustQuantity = item.AdjustQuantity,
                    ReasonCode = item.ReasonCode
                }).ToList() ?? new List<StockAdjustmentItemDetailResponse>()
            };
        }

        public static List<StockAdjustmentResponse> ToResponseList(this List<StockAdjustment> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
