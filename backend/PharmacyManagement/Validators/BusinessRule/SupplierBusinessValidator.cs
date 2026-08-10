using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class SupplierBusinessValidator
    {
        private readonly ISupplierRepository _repository;

        public SupplierBusinessValidator(ISupplierRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateSupplierNameNotExistsAsync(string supplierName)
        {
            if (await _repository.IsSupplierNameExistAsync(supplierName))
            {
                throw new BusinessException(
                    "Supplier name already exists.",
                    "SUPP001",
                    StatusCodes.Status409Conflict);
            }
        }
    }
}
