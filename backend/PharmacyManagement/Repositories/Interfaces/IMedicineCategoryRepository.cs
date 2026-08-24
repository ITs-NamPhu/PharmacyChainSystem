using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IMedicineCategoryRepository
    {
        Task<bool> IsCategoryNameExistAsync(string categoryName);
        Task<bool> HasMedicinesAsync(long categoryId);
        Task<MedicineCategory?> GetByIdAsync(long id);
        Task<List<MedicineCategory>> GetAllAsync();
        Task<List<MedicineCategory>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(MedicineCategory entity);
        void Update(MedicineCategory entity);
        void Delete(MedicineCategory entity);
        Task SaveChangesAsync();
    }
}
