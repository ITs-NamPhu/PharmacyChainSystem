using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class CustomerTypeBusinessValidator
    {
        private readonly ICustomerTypeRepository _repository;

        public CustomerTypeBusinessValidator(ICustomerTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateTypeNameNotExistsAsync(string typeName)
        {
            if (await _repository.IsTypeNameExistAsync(typeName))
            {
                throw new BusinessException(
                    "Customer type name already exists.",
                    "CUTYPE001",
                    StatusCodes.Status409Conflict);
            }
        }
    }
}
