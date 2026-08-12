using PharmacyManagement.DTOs.StockAdjustment;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
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
            if (request.Items.Count == 0)
                throw new BusinessException("Stock adjustment must contain at least one item.", "SA014", StatusCodes.Status400BadRequest);

            var stockTake = await _businessValidator.ValidateStockTakeCanAdjustAsync(request.StockTakeID);
            await _businessValidator.EnsureNoAdjustmentExistsAsync(request.StockTakeID);

            foreach (var item in request.Items)
            {
                var batch = await _businessValidator.ValidateBatchInWarehouseAsync(item.BatchID, stockTake.WarehouseID);
                await _businessValidator.ValidateStockTakeItemLinkAsync(item.StockTakeItemID, request.StockTakeID, item.BatchID);
                _businessValidator.ValidateStockNotNegative(batch, item.AdjustQuantity);
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
                    if (!await _repository.TryAdjustStockAsync(item.BatchID, item.AdjustQuantity))
                        throw new BusinessException(
                            $"Insufficient stock for batch ID {item.BatchID}.",
                            "SA012",
                            StatusCodes.Status400BadRequest);
                }

                stockTake.IsAdjusted = true;
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
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count, warehouseId);
            int numRecords = await _repository.CountAsync(warehouseId);
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new StockAdjustmentListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                StockAdjustments = entities.ToResponseList()
            };
        }
    }
}
