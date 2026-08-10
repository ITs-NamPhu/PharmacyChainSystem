using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class WarehouseBusinessValidator
    {
        private readonly IWarehouseRepository _repository;

        public WarehouseBusinessValidator(IWarehouseRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateBranchExistsAsync(long branchId)
        {
            if (!await _repository.IsBranchExistsAsync(branchId))
            {
                throw new BusinessException(
                    "Branch not found.",
                    "WH001",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
