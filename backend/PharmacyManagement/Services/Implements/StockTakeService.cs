using PharmacyManagement.DTOs.StockTake;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class StockTakeService : IStockTakeService
    {
        private readonly IStockTakeRepository _repository;
        private readonly StockTakeBusinessValidator _businessValidator;

        public StockTakeService(
            IStockTakeRepository repository,
            StockTakeBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<StockTakeDetailResponse> CreateAsync(CreateStockTakeRequest request, long userId)
        {
            if (request.Items.Count == 0)
                throw new BusinessException("Stock take must contain at least one item.", "ST009", StatusCodes.Status400BadRequest);

            await _businessValidator.ValidateWarehouseExistsAsync(request.WarehouseID);

            var batchIds = request.Items.Select(i => i.BatchID).ToList();
            var batches = await _businessValidator.ValidateBatchesInWarehouseAsync(batchIds, request.WarehouseID);

            var stockTake = request.ToEntity(userId);

            stockTake.StockTakeItem = request.Items
                .Select(item => item.ToEntity(0, batches[item.BatchID].QuantityInStock))
                .ToList();

            _businessValidator.ValidateBalanceEquation(stockTake.StockTakeItem);

            await _repository.AddAsync(stockTake);
            await _repository.SaveChangesAsync();

            return (await GetByIdAsync(stockTake.StockTakeID))!;
        }

        public async Task<StockTakeDetailResponse> UpdateAsync(long id, UpdateStockTakeRequest request, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            StatusTicketValidator.ValidateForUpdate(stockTake.Status, "kiểm kê");

            if (request.Items.Count == 0)
                throw new BusinessException("Stock take must contain at least one item.", "ST009", StatusCodes.Status400BadRequest);

            await _businessValidator.ValidateWarehouseExistsAsync(request.WarehouseID);

            var batchIds = request.Items.Select(i => i.BatchID).ToList();
            var batches = await _businessValidator.ValidateBatchesInWarehouseAsync(batchIds, request.WarehouseID);

            stockTake.WarehouseID = request.WarehouseID;
            stockTake.Note = request.Note ?? string.Empty;

            stockTake.StockTakeItem = request.Items
                .Select(item => item.ToEntity(0, batches[item.BatchID].QuantityInStock))
                .ToList();

            _businessValidator.ValidateBalanceEquation(stockTake.StockTakeItem);

            stockTake.Status = StatusTicket.PENDING;

            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();

            return (await GetByIdAsync(stockTake.StockTakeID))!;
        }

        public async Task ApproveAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            StatusTicketValidator.ValidateForApprove(stockTake.Status, "kiểm kê");
            StatusTicketValidator.ValidateCreatorNotApprover(stockTake.UserID, userId, "kiểm kê");

            stockTake.Status = StatusTicket.APPROVED;
            stockTake.ApprovedBy = userId;
            stockTake.ApprovedAt = DateTime.Now;

            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task RejectAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            StatusTicketValidator.ValidateForReject(stockTake.Status, "kiểm kê");

            stockTake.Status = StatusTicket.REJECTED;

            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task<StockTakeDetailResponse> CompleteAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            StatusTicketValidator.ValidateForComplete(stockTake.Status, "kiểm kê");

            var items = stockTake.StockTakeItem!.ToList();

            _businessValidator.ValidateBalanceEquation(items);

            var batchIds = items.Where(i => i.DifferenceQuantity != 0).Select(i => i.BatchID).ToList();
            var batches = await _repository.GetBatchesByIdsAsync(batchIds);

            await _repository.BeginTransactionAsync();
            try
            {
                var destroyItems = items.Where(i => i.DestroyQuantity > 0).ToList();
                if (destroyItems.Any())
                {
                    var destroyReceipt = new DestroyReceipt
                    {
                        WarehouseID = stockTake.WarehouseID,
                        UserID = userId,
                        StockTakeID = stockTake.StockTakeID,
                        Note = $"Tự động từ Kiểm kê #{stockTake.StockTakeID}",
                        CreatedAt = DateTime.Now,
                        Status = StatusTicket.APPROVED,
                        ApprovedBy = userId,
                        ApprovedAt = DateTime.Now,
                        DestroyReceiptItem = destroyItems.Select(di => new DestroyReceiptItem
                        {
                            BatchID = di.BatchID,
                            StockTakeItemID = di.StockTakeItemID,
                            Quantity = di.DestroyQuantity,
                            UnitCost = batches[di.BatchID].GoodsReceiptItem?.UnitCost ?? 0,
                            ReasonCode = "StockTake-AutoDestroy"
                        }).ToList()
                    };
                    await _repository.AddDestroyReceiptAsync(destroyReceipt);
                }

                var adjustItems = items.Where(i => i.AdjustQuantity != 0).ToList();
                if (adjustItems.Any())
                {
                    var adjustment = new StockAdjustment
                    {
                        WarehouseID = stockTake.WarehouseID,
                        UserID = userId,
                        StockTakeID = stockTake.StockTakeID,
                        Note = $"Tự động từ Kiểm kê #{stockTake.StockTakeID}",
                        CreatedAt = DateTime.Now,
                        Status = StatusTicket.APPROVED,
                        ApprovedBy = userId,
                        ApprovedAt = DateTime.Now,
                        StockAdjustmentItem = adjustItems.Select(ai => new StockAdjustmentItem
                        {
                            BatchID = ai.BatchID,
                            StockTakeItemID = ai.StockTakeItemID,
                            AdjustQuantity = ai.DifferenceQuantity < 0
                                ? -ai.AdjustQuantity
                                : ai.AdjustQuantity,
                            ReasonCode = "StockTake-AutoAdjust"
                        }).ToList()
                    };
                    await _repository.AddStockAdjustmentAsync(adjustment);
                }

                foreach (var item in items)
                {
                    if (item.DifferenceQuantity != 0 && batches.TryGetValue(item.BatchID, out var batch))
                    {
                        batch.QuantityInStock = item.ActualQuantity;
                    }
                }

                stockTake.Status = StatusTicket.COMPLETE;
                _repository.Update(stockTake);

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }

            return (await GetByIdAsync(stockTake.StockTakeID))!;
        }

        public async Task CancelAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            if (stockTake.Status == StatusTicket.COMPLETE)
                throw new BusinessException(
                    "A completed stock take cannot be cancelled.",
                    "ST007",
                    StatusCodes.Status400BadRequest);

            if (stockTake.Status == StatusTicket.REJECTED)
                throw new BusinessException(
                    "Stock take has already been rejected.",
                    "ST008",
                    StatusCodes.Status400BadRequest);

            stockTake.Status = StatusTicket.REJECTED;
            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            StatusTicketValidator.ValidateForDelete(stockTake.Status, "kiểm kê");

            _repository.Delete(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task<StockTakeDetailResponse?> GetByIdAsync(long id)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            return stockTake?.ToDetailResponse();
        }

        public async Task<StockTakeListResponse> GetAllAsync(int page, int count, long warehouseId)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                (skip, take) => _repository.GetAllAsync(skip, take, warehouseId),
                () => _repository.CountAsync(warehouseId),
                page, count);

            return new StockTakeListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                StockTakes = paged.Items.ToResponseList()
            };
        }
    }
}
