using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class GoodsReceiptBusinessValidator
    {
        private readonly IGoodsReceiptRepository _repository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IUnitRepository _unitRepository;

        public GoodsReceiptBusinessValidator(
            IGoodsReceiptRepository repository,
            ISupplierRepository supplierRepository,
            IMedicineRepository medicineRepository,
            IUnitRepository unitRepository)
        {
            _repository = repository;
            _supplierRepository = supplierRepository;
            _medicineRepository = medicineRepository;
            _unitRepository = unitRepository;
        }

        public async Task ValidateSupplierExistsAsync(long supplierId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            if (supplier == null)
            {
                throw new BusinessException(
                    "Supplier not found.",
                    "GR010",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateMedicineExistsAsync(long medicineId)
        {
            if (!await _medicineRepository.IsExistsAsync(medicineId))
            {
                throw new BusinessException(
                    "Medicine not found.",
                    "GR011",
                    StatusCodes.Status404NotFound);
            }
        }

        public async Task ValidateUnitExistsAsync(long unitId)
        {
            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null)
            {
                throw new BusinessException(
                    "Unit not found.",
                    "GR012",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
