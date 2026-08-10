using PharmacyManagement.DTOs.MedicineCategory;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IMedicineCategoryService
    {
        Task<MedicineCategoryResponse> CreateAsync(CreateMedicineCategoryRequest request);
        Task<MedicineCategoryResponse> UpdateAsync(long id, UpdateMedicineCategoryRequest request);
        Task DeleteAsync(long id);
        Task<MedicineCategoryResponse?> GetByIdAsync(long id);
        Task<List<MedicineCategoryResponse>> GetAllCategoryAsync();
        Task<MedicineCategoryListResponse> GetAllAsync(int page, int count);
    }
}
