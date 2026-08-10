using PharmacyManagement.DTOs.Branch;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repository;
        private readonly BranchBusinessValidator _businessValidator;

        public BranchService(IBranchRepository repository, BranchBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<BranchResponse> CreateAsync(CreateBranchRequest request)
        {
            await _businessValidator.ValidatePriceListExistsAsync(request.PriceListID);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<BranchResponse> UpdateAsync(long id, UpdateBranchRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Branch not found.", "BRANCH004", StatusCodes.Status404NotFound);

            if (request.PriceListID.HasValue)
                await _businessValidator.ValidatePriceListExistsAsync(request.PriceListID.Value);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Branch not found.", "BRANCH004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<BranchResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<List<BranchResponse>> GetAllBranchAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<BranchListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new BranchListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                Branches = entities.ToResponseList()
            };
        }
    }
}
