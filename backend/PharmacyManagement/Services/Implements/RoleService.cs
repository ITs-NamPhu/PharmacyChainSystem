using PharmacyManagement.DTOs.Role;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly RoleBusinessValidator _businessValidator;

        public RoleService(IRoleRepository repository, RoleBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<RoleResponse> CreateAsync(CreateRoleRequest request)
        {
            await _businessValidator.ValidateRoleNameNotExistsAsync(request.RoleName);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<RoleResponse> UpdateAsync(long id, UpdateRoleRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Role not found.", "ROLE004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Role not found.", "ROLE004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<RoleResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<RoleListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new RoleListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                Roles = entities.ToResponseList()
            };
        }

        public async Task<List<RoleResponse>> GetAllRoleAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }
    }
}
