using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class ManufacturerBusinessValidator
    {
        private readonly IManufacturerRepository _repository;

        public ManufacturerBusinessValidator(IManufacturerRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateManufacturerNameNotExistsAsync(string manufacturerName)
        {
            if (await _repository.IsManufacturerNameExistAsync(manufacturerName))
            {
                throw new BusinessException(
                    "Manufacturer name already exists.",
                    "MFR001",
                    StatusCodes.Status409Conflict);
            }
        }
    }
}
