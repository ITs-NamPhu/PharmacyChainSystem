using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class UnitBusinessValidator
    {
        private readonly IUnitRepository _repository;

        public UnitBusinessValidator(IUnitRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateUnitNameNotExistsAsync(string unitName)
        {
            if (await _repository.IsUnitNameExistAsync(unitName))
            {
                throw new BusinessException(
                    "Unit name already exists.",
                    "UNIT001",
                    StatusCodes.Status409Conflict);
            }
        }
    }
}
