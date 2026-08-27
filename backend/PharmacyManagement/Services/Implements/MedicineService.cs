using PharmacyManagement.DTOs.Medicine;
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
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _repository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly MedicineBusinessValidator _businessValidator;

        public MedicineService(
            IMedicineRepository repository,
            IWarehouseRepository warehouseRepository,
            MedicineBusinessValidator businessValidator)
        {
            _repository = repository;
            _warehouseRepository = warehouseRepository;
            _businessValidator = businessValidator;
        }

        public async Task<MedicineResponse> CreateAsync(CreateMedicineRequest request)
        {
            await _businessValidator.ValidateMedicineNameNotExistsAsync(request.MedicineName);
            await _businessValidator.ValidateCategoryExistsAsync(request.CategoryID);
            await _businessValidator.ValidateManufacturerExistsAsync(request.ManufacturerID);
            await _businessValidator.ValidateUnitExistsAsync(request.BaseUnitID);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<MedicineResponse> UpdateAsync(long id, UpdateMedicineRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Medicine not found.", "MED004", StatusCodes.Status404NotFound);

            if (request.CategoryID.HasValue)
                await _businessValidator.ValidateCategoryExistsAsync(request.CategoryID.Value);
            if (request.ManufacturerID.HasValue)
                await _businessValidator.ValidateManufacturerExistsAsync(request.ManufacturerID.Value);
            if (request.BaseUnitID.HasValue)
                await _businessValidator.ValidateUnitExistsAsync(request.BaseUnitID.Value);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Medicine not found.", "MED004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<MedicineResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<MedicineListResponse> GetAllAsync(MedicineFilterDto filter, long branchId)
        {
            var threshold = filter.StockThreshold ?? 10m;

            var query = _repository.GetQuery()
                .AttachStock(_warehouseRepository.GetBatchQuery(), branchId)
                .FilterByKeyword(filter.Keyword)
                .FilterByCategory(filter.CategoryID)
                .FilterByManufacturer(filter.ManufacturerID)
                .FilterByStockStatus(filter.StockStatus, threshold)
                .ApplySort(filter.SortBy, filter.IsDescending);

            var projectedQuery = query.Select(w => new MedicineResponse
            {
                MedicineID = w.Medicine.MedicineID,
                MedicineName = w.Medicine.MedicineName,
                DefaultRetailPrice = w.Medicine.DefaultRetailPrice,
                DefaultWholesalePrice = w.Medicine.DefaultWholesalePrice,
                VATPercent = w.Medicine.VATPercent,
                CategoryID = w.Medicine.CategoryID,
                CategoryName = w.Medicine.MedicineCategory != null
                    ? w.Medicine.MedicineCategory.CategoryName : null,
                ManufacturerID = w.Medicine.ManufacturerID,
                ManufacturerName = w.Medicine.ManuFacturer != null
                    ? w.Medicine.ManuFacturer.ManufacturerName : null,
                BaseUnitID = w.Medicine.BaseUnitID,
                UnitName = w.Medicine.Unit != null ? w.Medicine.Unit.UnitName : null,
                TotalStock = w.TotalStock
            });

            var paged = await projectedQuery.ToPagedResultAsync(filter);

            return new MedicineListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Medicines = paged.Items
            };
        }

        public async Task<List<MedicineResponse>> GetAllMedicineAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }
    }
}
