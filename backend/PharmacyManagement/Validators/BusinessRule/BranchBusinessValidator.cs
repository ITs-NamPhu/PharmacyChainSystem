using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class BranchBusinessValidator
    {
        private readonly IBranchRepository _repository;

        public BranchBusinessValidator(IBranchRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidatePriceListExistsAsync(long priceListId)
        {
            if (!await _repository.IsPriceListExistsAsync(priceListId))
            {
                throw new BusinessException(
                    "Price list not found.",
                    "BRANCH001",
                    StatusCodes.Status404NotFound);
            }
        }
    }
}
