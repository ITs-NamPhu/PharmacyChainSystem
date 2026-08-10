using PharmacyManagement.DTOs.UnitConversion;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IUnitConversionService
    {
        Task<UnitConversionResponse> CreateAsync(CreateUnitConversionRequest request);
        Task<UnitConversionResponse> UpdateAsync(long id, UpdateUnitConversionRequest request);
        Task DeleteAsync(long id);
        Task<UnitConversionResponse?> GetByIdAsync(long id);
        Task<List<UnitConversionResponse>> GetAllUnitConversionAsync();
        Task<UnitConversionListResponse> GetAllAsync(int page, int count);

        Task<List<UnitConversion_MedicineResponse>> GetListUnitConversion_MedicineAsync();
    }
}
