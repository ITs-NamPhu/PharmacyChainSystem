using PharmacyManagement.DTOs.Medicine;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IMedicineService
    {
        Task<MedicineResponse> CreateAsync(CreateMedicineRequest request);
        Task<MedicineResponse> UpdateAsync(long id, UpdateMedicineRequest request);
        Task DeleteAsync(long id);
        Task<MedicineResponse?> GetByIdAsync(long id);
        Task<MedicineListResponse> GetAllAsync(MedicineFilterDto filter, long branchId);
        Task<List<MedicineResponse>> GetAllMedicineAsync();
    }
}
