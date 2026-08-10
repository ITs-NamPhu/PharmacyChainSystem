using PharmacyManagement.DTOs.Manufacturer;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<ManufacturerResponse> CreateAsync(CreateManufacturerRequest request);
        Task<ManufacturerResponse> UpdateAsync(long id, UpdateManufacturerRequest request);
        Task DeleteAsync(long id);
        Task<ManufacturerResponse?> GetByIdAsync(long id);
        Task<List<ManufacturerResponse>> GetAllManufacturerAsync();
        Task<ManufacturerListResponse> GetAllAsync(int page, int count);
    }
}
