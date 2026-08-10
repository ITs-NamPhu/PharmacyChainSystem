using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IMedicineRepository
    {
        Task<bool> IsMedicineNameExistAsync(string medicineName);
        Task<bool> IsExistsAsync(long id);
        Task<bool> IsCategoryExistsAsync(long categoryId);
        Task<bool> IsManufacturerExistsAsync(long manufacturerId);
        Task<bool> IsUnitExistsAsync(long unitId);
        Task<Models.Medicine?> GetByIdAsync(long id);
        Task<List<Models.Medicine>> GetAllAsync(int skip, int take);
        Task<List<Models.Medicine>> GetAllAsync();
        Task<int> CountAsync();
        Task AddAsync(Models.Medicine entity);
        void Update(Models.Medicine entity);
        void Delete(Models.Medicine entity);
        Task SaveChangesAsync();
    }
}
