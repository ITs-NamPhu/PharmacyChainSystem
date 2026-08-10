using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class UnitConversionBusinessValidator
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMedicineRepository _medicineRepository;

        public UnitConversionBusinessValidator(IUnitRepository unitRepository, IMedicineRepository medicineRepository)
        {
            _unitRepository = unitRepository;
            _medicineRepository = medicineRepository;
        }

        public async Task ValidateUnitExistsAsync(long unitId)
        {
            if (!await _unitRepository.IsExistsAsync(unitId))
            {
                throw new BusinessException(
                    "Unit not found.",
                    "UNITCONVERSION001",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateMedicineExistsAsync(long medicineId)
        {
            if (!await _medicineRepository.IsExistsAsync(medicineId))
            {
                throw new BusinessException(
                    "Medicine not found.",
                    "UNITCONVERSION002",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
