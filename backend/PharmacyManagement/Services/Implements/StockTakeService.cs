using PharmacyManagement.DTOs.StockTake;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
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

            stockTake.IsBalance = stockTake.StockTakeItem.All(i => i.DifferenceQuantity == 0)
                ? StockTakeResult.Balanced
                : StockTakeResult.Difference;

            stockTake.IsAdjust = stockTake.StockTakeItem.Any(i => i.IsAdjust);
            stockTake.IsDestroy = stockTake.StockTakeItem.Any(i => i.IsDestroy);



            await _repository.AddAsync(stockTake);
            await _repository.SaveChangesAsync();

            return (await GetByIdAsync(stockTake.StockTakeID))!;
        }

        public async Task<StockTakeDetailResponse> CompleteAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            if (stockTake.Status != StockTakeStatus.Draft)
                throw new BusinessException(
                    "Only draft stock takes can be completed.",
                    "ST005",
                    StatusCodes.Status400BadRequest);

            stockTake.Status = StockTakeStatus.Completed;

            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();

            return (await GetByIdAsync(stockTake.StockTakeID))!;
        }

        public async Task CancelAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            if (stockTake.Status == StockTakeStatus.Completed)
                throw new BusinessException(
                    "A completed stock take cannot be cancelled.",
                    "ST007",
                    StatusCodes.Status400BadRequest);

            if (stockTake.Status == StockTakeStatus.Cancelled)
                throw new BusinessException(
                    "Stock take has already been cancelled.",
                    "ST008",
                    StatusCodes.Status400BadRequest);

            stockTake.Status = StockTakeStatus.Cancelled;
            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task ApproveAsync(long id, long userId)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            if (stockTake == null)
                throw new BusinessException("Stock take not found.", "ST004", StatusCodes.Status404NotFound);

            stockTake.ApprovedBy = userId;
            stockTake.ApprovedAt = DateTime.Now;
            _repository.Update(stockTake);
            await _repository.SaveChangesAsync();
        }

        public async Task<StockTakeDetailResponse?> GetByIdAsync(long id)
        {
            var stockTake = await _repository.GetByIdAsync(id);
            return stockTake?.ToDetailResponse();
        }

        public async Task<StockTakeListResponse> GetAllAsync(int page, int count, long warehouseId)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count, warehouseId);
            int numRecords = await _repository.CountAsync(warehouseId);
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new StockTakeListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                StockTakes = entities.ToResponseList()
            };
        }
    }
}
