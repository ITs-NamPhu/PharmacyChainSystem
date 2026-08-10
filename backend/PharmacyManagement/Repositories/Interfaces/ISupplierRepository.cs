using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface ISupplierRepository
    {
        Task<bool> IsSupplierNameExistAsync(string supplierName);
        Task<Supplier?> GetByIdAsync(long id);
        Task<List<Supplier>> GetAllAsync(int skip, int take);
        Task<int> CountAsync();
        Task AddAsync(Supplier entity);
        void Update(Supplier entity);
        void Delete(Supplier entity);
        Task SaveChangesAsync();
    }
}
