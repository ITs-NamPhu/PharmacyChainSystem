using PharmacyManagement.DTOs.StockAdjustment;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IStockAdjustmentRepository _repository;
        private readonly StockAdjustmentBusinessValidator _businessValidator;

        public StockAdjustmentService(
            IStockAdjustmentRepository repository,
            StockAdjustmentBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<StockAdjustmentDetailResponse> CreateAsync(CreateStockAdjustmentRequest request, long userId)
        {

            // 1. Validate request items
            if (request.Items.Count == 0)
                throw new BusinessException("Stock adjustment must contain at least one item.", "SA014", StatusCodes.Status400BadRequest);

            // Validate stock take and ensure no existing adjustment
            var stockTake = await _businessValidator.ValidateStockTakeCanAdjustAsync(request.StockTakeID);

            await _businessValidator.EnsureNoAdjustmentExistsAsync(request.StockTakeID);


            // Validate batches and stock quantities
            var batches = await _businessValidator.ValidateBatchesInWarehouseAsync(
                request.Items.Select(i => i.BatchID),
                stockTake.WarehouseID);

            foreach (var item in request.Items)
            {
                _businessValidator.ValidateStockTakeItemLink(item.StockTakeItemID, stockTake.StockTakeItem!, request.StockTakeID, item.BatchID);
                _businessValidator.ValidateStockNotNegative(batches[item.BatchID], item.AdjustQuantity);
            }

            var adjustment = request.ToEntity(userId);

            adjustment.WarehouseID = stockTake.WarehouseID;
            adjustment.StockAdjustmentItem = request.Items
                .Select(item => item.ToEntity(0))
                .ToList();



            await _repository.BeginTransactionAsync();
            try
            {
                await _repository.AddAsync(adjustment);

                foreach (var item in adjustment.StockAdjustmentItem!)
                {
                    if (!batches.TryGetValue(item.BatchID, out var batch)
                        || batch.QuantityInStock + item.AdjustQuantity < 0)
                        throw new BusinessException(
                            $"Insufficient stock for batch ID {item.BatchID}.",
                            "SA012",
                            StatusCodes.Status400BadRequest);

                    batch.QuantityInStock += item.AdjustQuantity;
                }

                await _repository.SaveChangesAsync();
                await _repository.CommitTransactionAsync();
            }
            catch
            {
                await _repository.RollbackTransactionAsync();
                throw;
            }

            return (await GetByIdAsync(adjustment.StockAdjustmentID))!;
        }

        public async Task ApproveAsync(long id, long userId)
        {
            var adjustment = await _repository.GetByIdAsync(id);
            if (adjustment == null)
                throw new BusinessException("Stock adjustment not found.", "SA013", StatusCodes.Status404NotFound);

            adjustment.ApprovedBy = userId;
            adjustment.ApprovedAt = DateTime.Now;
            await _repository.SaveChangesAsync();
        }

        public async Task<StockAdjustmentDetailResponse?> GetByIdAsync(long id)
        {
            var adjustment = await _repository.GetByIdAsync(id);
            return adjustment?.ToDetailResponse();
        }

        public async Task<StockAdjustmentListResponse> GetAllAsync(int page, int count, long warehouseId)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                (skip, take) => _repository.GetAllAsync(skip, take, warehouseId),
                () => _repository.CountAsync(warehouseId),
                page, count);

            return new StockAdjustmentListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                StockAdjustments = paged.Items.ToResponseList()
            };
        }
    }
}
