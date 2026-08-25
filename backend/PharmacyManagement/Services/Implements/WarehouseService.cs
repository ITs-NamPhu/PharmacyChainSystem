using PharmacyManagement.DTOs.Batch;
using PharmacyManagement.DTOs.Warehouse;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Extensions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repository;
        private readonly WarehouseBusinessValidator _businessValidator;

        public WarehouseService(IWarehouseRepository repository, WarehouseBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, long branchId)
        {
            await _businessValidator.ValidateBranchExistsAsync(branchId);

            var entity = request.ToEntity(branchId);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<WarehouseResponse> UpdateAsync(long id, UpdateWarehouseRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Warehouse not found.", "WH004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Warehouse not found.", "WH004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<WarehouseResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<WarehouseListResponse> GetAllAsync(int page, int count)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                _repository.GetAllAsync, _repository.CountAsync, page, count);

            return new WarehouseListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Warehouses = paged.Items.ToResponseList()
            };
        }

        public async Task<WarehouseListResponse> GetByBranchAsync(long branchId, int page, int count)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                (skip, take) => _repository.GetByBranchAsync(branchId, skip, take),
                () => _repository.CountByBranchAsync(branchId),
                page, count);

            return new WarehouseListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Warehouses = paged.Items.ToResponseList()
            };
        }

        public async Task<WarehouseResponse?> GetByBranchIdAsync(long branchId)
        {
            var entity = await _repository.GetByBranchIdAsync(branchId);
            return entity?.ToResponse();
        }

        public async Task<WarehouseBatchListResponse> GetBatchesAsync(BatchFilterDto filter, long branchId)
        {
            var query = _repository.GetBatchQuery()
                .FilterByBranch(branchId)
                .FilterByWarehouse(filter.WarehouseID)
                .FilterByExpiryStatus(filter.ExpiryStatus)
                .FilterByKeyword(filter.Keyword)
                .ApplySort(filter.SortBy, filter.IsDescending);

            var paged = await query.ToPagedResultAsync(filter);

            return new WarehouseBatchListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Batches = paged.Items.Select(b => new WarehouseBatchResponse
                {
                    BatchID = b.BatchID,
                    MedicineID = b.GoodsReceiptItem != null ? b.GoodsReceiptItem.MedicineID : 0,
                    MedicineName = b.GoodsReceiptItem != null && b.GoodsReceiptItem.Medicine != null
                        ? b.GoodsReceiptItem.Medicine.MedicineName : string.Empty,
                    UnitName = b.GoodsReceiptItem != null ? b.GoodsReceiptItem.UnitName : string.Empty,
                    UnitCost = b.GoodsReceiptItem != null ? b.GoodsReceiptItem.UnitCost : 0,
                    QuantityReceived = b.QuantityReceived,
                    QuantityInStock = b.QuantityInStock,
                    ManufactureDate = b.ManufactureDate,
                    ExpiryDate = b.ExpiryDate,
                    Note = b.Note
                }).ToList()
            };
        }
    }
}
