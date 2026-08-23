using PharmacyManagement.DTOs.Medicine;
using PharmacyManagement.Exceptions;
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
        private readonly MedicineBusinessValidator _businessValidator;

        public MedicineService(IMedicineRepository repository, MedicineBusinessValidator businessValidator)
        {
            _repository = repository;
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

        public async Task<MedicineListResponse> GetAllAsync(int page, int count)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                _repository.GetAllAsync, _repository.CountAsync, page, count);

            return new MedicineListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                Medicines = paged.Items.ToResponseList()
            };
        }

        public async Task<List<MedicineResponse>> GetAllMedicineAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }
    }
}
