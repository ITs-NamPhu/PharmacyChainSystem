using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IUnitConversionRepository
    {
        Task<UnitConversion?> GetByIdAsync(long id);
        Task<List<UnitConversion>> GetAllAsync();
        Task<List<UnitConversion>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(UnitConversion entity);
        void Update(UnitConversion entity);
        void Delete(UnitConversion entity);
        Task SaveChangesAsync();
        Task<UnitConversion?> GetByMedicineAndUnitAsync(long medicineId, long unitId);
        Task<List<UnitConversion>> GetByMedicineIdsAsync(IEnumerable<long> medicineIds);
    }
}
