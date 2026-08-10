using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class MedicineBusinessValidator
    {
        private readonly IMedicineRepository _repository;

        public MedicineBusinessValidator(IMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateMedicineNameNotExistsAsync(string medicineName)
        {
            if (await _repository.IsMedicineNameExistAsync(medicineName))
            {
                throw new BusinessException(
                    "Medicine name already exists.",
                    "MED001",
                    StatusCodes.Status409Conflict);
            }
        }

        public async Task ValidateCategoryExistsAsync(long categoryId)
        {
            if (!await _repository.IsCategoryExistsAsync(categoryId))
            {
                throw new BusinessException(
                    "Medicine category not found.",
                    "MED002",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateManufacturerExistsAsync(long manufacturerId)
        {
            if (!await _repository.IsManufacturerExistsAsync(manufacturerId))
            {
                throw new BusinessException(
                    "Manufacturer not found.",
                    "MED003",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateUnitExistsAsync(long unitId)
        {
            if (!await _repository.IsUnitExistsAsync(unitId))
            {
                throw new BusinessException(
                    "Unit not found.",
                    "MED004",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
