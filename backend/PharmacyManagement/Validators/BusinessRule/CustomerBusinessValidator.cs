using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class CustomerBusinessValidator
    {
        private readonly ICustomerRepository _repository;

        public CustomerBusinessValidator(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateCustomerTypeExistsAsync(long customerTypeId)
        {
            if (!await _repository.IsCustomerTypeExistsAsync(customerTypeId))
            {
                throw new BusinessException(
                    "Customer type not found.",
                    "CUST001",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
