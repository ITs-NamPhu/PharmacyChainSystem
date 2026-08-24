using PharmacyManagement.DTOs.MedicineCategory;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Mappers;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Validators.BusinessRule;
using Microsoft.AspNetCore.Http;

namespace PharmacyManagement.Services.Implements
{
    public class MedicineCategoryService : IMedicineCategoryService
    {
        private readonly IMedicineCategoryRepository _repository;
        private readonly MedicineCategoryBusinessValidator _businessValidator;

        public MedicineCategoryService(IMedicineCategoryRepository repository, MedicineCategoryBusinessValidator businessValidator)
        {
            _repository = repository;
            _businessValidator = businessValidator;
        }

        public async Task<MedicineCategoryResponse> CreateAsync(CreateMedicineCategoryRequest request)
        {
            await _businessValidator.ValidateCategoryNameNotExistsAsync(request.CategoryName);

            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<MedicineCategoryResponse> UpdateAsync(long id, UpdateMedicineCategoryRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Medicine category not found.", "MCAT004", StatusCodes.Status404NotFound);

            request.ApplyTo(entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new BusinessException("Medicine category not found.", "MCAT004", StatusCodes.Status404NotFound);

            // chỉ cho xóa khi không còn bảng con (Medicine) tham chiếu
            await _businessValidator.ValidateCategoryNotInUseAsync(id);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<MedicineCategoryResponse?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity?.ToResponse();
        }

        public async Task<List<MedicineCategoryResponse>> GetAllCategoryAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.ToResponseList();
        }

        public async Task<MedicineCategoryListResponse> GetAllAsync(int page, int count)
        {
            int skip = (page - 1) * count;
            var entities = await _repository.GetAllAsync(skip, count);
            int numRecords = await _repository.CountAsync();
            float totalPage = (float)Math.Ceiling((double)numRecords / count);

            return new MedicineCategoryListResponse
            {
                NumRecords = numRecords,
                TotalPage = totalPage,
                MedicineCategories = entities.ToResponseList()
            };
        }
    }
}
