using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IManufacturerRepository
    {
        Task<bool> IsManufacturerNameExistAsync(string manufacturerName);
        Task<ManuFacturer?> GetByIdAsync(long id);
        Task<List<ManuFacturer>> GetAllAsync();
        Task<List<ManuFacturer>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(ManuFacturer entity);
        void Update(ManuFacturer entity);
        void Delete(ManuFacturer entity);
        Task SaveChangesAsync();
    }
}
