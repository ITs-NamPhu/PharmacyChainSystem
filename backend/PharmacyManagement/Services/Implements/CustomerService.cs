using PharmacyManagement.DTOs.Customer;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly CustomerBusinessValidator _businessValidator;

        public CustomerService(ICustomerRepository repository, CustomerBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            await _businessValidator.ValidateCustomerTypeExistsAsync(request.CustomerTypeID);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<CustomerResponse> UpdateAsync(long id, UpdateCustomerRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer not found.", "CUST004", StatusCodes.Status404NotFound);

            await _businessValidator.ValidateCustomerTypeExistsAsync(request.CustomerTypeID);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Customer not found.", "CUST004", StatusCodes.Status404NotFound);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<CustomerResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<CustomerListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new CustomerListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                Customers = entities.ToResponseList()
            };
        }

        public async Task<List<CustomerResponse>> GetAllCustomerAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }
    }
}
