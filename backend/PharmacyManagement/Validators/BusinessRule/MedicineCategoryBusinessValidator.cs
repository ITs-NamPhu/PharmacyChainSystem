using PharmacyManagement.Exceptions;
using PharmacyManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Validators.BusinessRule
{
    public class MedicineCategoryBusinessValidator
    {
        private readonly IMedicineCategoryRepository _repository;

        public MedicineCategoryBusinessValidator(IMedicineCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateCategoryNameNotExistsAsync(string categoryName)
        {
            if (await _repository.IsCategoryNameExistAsync(categoryName))
            {
                throw new BusinessException(
                    "Medicine category name already exists.",
                    "MCAT001",
                    StatusCodes.Status409Conflict);
            }
        }

        public async Task ValidateCategoryNotInUseAsync(long categoryId)
        {
            if (await _repository.HasMedicinesAsync(categoryId))
            {
                throw new BusinessException(
                    "Cannot delete medicine category because it is used by medicines.",
                    "MCAT005",
                    StatusCodes.Status400BadRequest);
            }
        }
    }
}
