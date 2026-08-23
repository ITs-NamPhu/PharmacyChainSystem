using PharmacyManagement.DTOs.CustomerType;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.share;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ICustomerTypeRepository _repository;
        private readonly CustomerTypeBusinessValidator _businessValidator;

        public CustomerTypeService(ICustomerTypeRepository repository, CustomerTypeBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<CustomerTypeResponse> CreateAsync(CreateCustomerTypeRequest request)
        {
            await _businessValidator.ValidateTypeNameNotExistsAsync(request.TypeName);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<CustomerTypeResponse> UpdateAsync(long id, UpdateCustomerTypeRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer type not found.", "CUTYPE004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer type not found.", "CUTYPE004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<CustomerTypeResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<List<CustomerTypeResponse>> GetAllCustomerTypeAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<CustomerTypeListResponse> GetAllAsync(int page, int count)
        {
            var paged = await PaginationHelper.GetPagedAsync(
                _repository.GetAllAsync, _repository.CountAsync, page, count);

            return new CustomerTypeListResponse
            {
                NumRecords = paged.NumRecords,
                TotalPage = paged.TotalPage,
                CustomerTypes = paged.Items.ToResponseList()
            };
        }
    }
}
