using PharmacyManagement.DTOs.Unit;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repository;
        private readonly UnitBusinessValidator _businessValidator;

        public UnitService(IUnitRepository repository, UnitBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<UnitResponse> CreateAsync(CreateUnitRequest request)
        {
            await _businessValidator.ValidateUnitNameNotExistsAsync(request.UnitName);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<UnitResponse> UpdateAsync(long id, UpdateUnitRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Unit not found.", "UNIT004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Unit not found.", "UNIT004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<UnitResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<List<UnitResponse>> GetAllUnitAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<UnitListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new UnitListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                Units = entities.ToResponseList()
            };
        }
    }
}
