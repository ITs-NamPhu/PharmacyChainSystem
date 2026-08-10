using PharmacyManagement.DTOs.UnitConversion;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class UnitConversionService : IUnitConversionService
    {
        private readonly IUnitConversionRepository _repository;
        private readonly UnitConversionBusinessValidator _businessValidator;

        public UnitConversionService(IUnitConversionRepository repository, UnitConversionBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<UnitConversionResponse> CreateAsync(CreateUnitConversionRequest request)
        {
            await _businessValidator.ValidateUnitExistsAsync(request.UnitID);
            await _businessValidator.ValidateMedicineExistsAsync(request.MedicineID);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<UnitConversionResponse> UpdateAsync(long id, UpdateUnitConversionRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Unit conversion not found.", "UNITCONVERSION004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Unit conversion not found.", "UNITCONVERSION004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<UnitConversionResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<List<UnitConversionResponse>> GetAllUnitConversionAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<UnitConversionListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new UnitConversionListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                UnitConversions = entities.ToResponseList()
            };
        }

        public async Task<List<UnitConversion_MedicineResponse>> GetListUnitConversion_MedicineAsync()
        {
            var entities = await _repository.GetAllAsync();

            return entities.Select(e => e.ToResponse_MedicineName()).ToList();
        }
    }
}
