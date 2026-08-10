using PharmacyManagement.DTOs.Unit;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IUnitService
    {
        Task<UnitResponse> CreateAsync(CreateUnitRequest request);
        Task<UnitResponse> UpdateAsync(long id, UpdateUnitRequest request);
        Task DeleteAsync(long id);
        Task<UnitResponse?> GetByIdAsync(long id);
        Task<List<UnitResponse>> GetAllUnitAsync();
        Task<UnitListResponse> GetAllAsync(int page, int count);
    }
}
