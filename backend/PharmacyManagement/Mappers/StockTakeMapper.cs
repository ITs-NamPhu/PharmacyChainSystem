using PharmacyManagement.DTOs.StockTake;
using PharmacyManagement.Models;

namespace PharmacyManagement.Mappers
{
    public static class StockTakeMapper
    {
        public static StockTake ToEntity(this CreateStockTakeRequest request, long userId)
        {
            return new StockTake
            {
                WarehouseID = request.WarehouseID,
                UserID = userId,
                CreatedAt = DateTime.Now,
                Note = request.Note ?? string.Empty,
                IsBalance = StockTakeResult.Difference,
                Status = StockTakeStatus.Draft,
                IsAdjusted = false
            };
        }

        public static StockTakeItem ToEntity(
            this CreateStockTakeItemRequest request,
            long stockTakeId,
            decimal systemQuantity)
        {
            return new StockTakeItem
            {
                StockTakeID = stockTakeId,
                BatchID = request.BatchID,
                SystemQuantity = systemQuantity,
                ActualQuantity = request.ActualQuantity,
                DifferenceQuantity = request.ActualQuantity - systemQuantity
            };
        }

        public static StockTakeResponse ToResponse(this StockTake entity)
        {
            return new StockTakeResponse
            {
                StockTakeID = entity.StockTakeID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                Note = entity.Note,
                IsBalance = entity.IsBalance.ToString(),
                Status = entity.Status.ToString(),
                IsAdjusted = entity.IsAdjusted,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                ItemCount = entity.StockTakeItem?.Count ?? 0
            };
        }

        public static StockTakeDetailResponse ToDetailResponse(this StockTake entity)
        {
            return new StockTakeDetailResponse
            {
                StockTakeID = entity.StockTakeID,
                WarehouseID = entity.WarehouseID,
                WarehouseName = entity.WareHouse?.WarehouseName ?? string.Empty,
                UserID = entity.UserID,
                UserName = entity.User?.FullName ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                Note = entity.Note,
                IsBalance = entity.IsBalance.ToString(),
                Status = entity.Status.ToString(),
                IsAdjusted = entity.IsAdjusted,
                ApprovedBy = entity.ApprovedBy,
                ApprovedAt = entity.ApprovedAt,
                Items = entity.StockTakeItem?.Select(item => new StockTakeItemDetailResponse
                {
                    StockTakeItemID = item.StockTakeItemID,
                    BatchID = item.BatchID,
                    MedicineID = item.Batch?.GoodsReceiptItem?.MedicineID ?? 0,
                    MedicineName = item.Batch?.GoodsReceiptItem?.Medicine?.MedicineName ?? string.Empty,
                    UnitName = item.Batch?.GoodsReceiptItem?.UnitName ?? string.Empty,
                    SystemQuantity = item.SystemQuantity,
                    ActualQuantity = item.ActualQuantity,
                    DifferenceQuantity = item.DifferenceQuantity
                }).ToList() ?? new List<StockTakeItemDetailResponse>()
            };
        }

        public static List<StockTakeResponse> ToResponseList(this List<StockTake> entities)
        {
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
