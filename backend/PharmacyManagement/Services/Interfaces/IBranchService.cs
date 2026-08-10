using PharmacyManagement.DTOs.Branch;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IBranchService
    {
        Task<BranchResponse> CreateAsync(CreateBranchRequest request);
        Task<BranchResponse> UpdateAsync(long id, UpdateBranchRequest request);
        Task DeleteAsync(long id);
        Task<BranchResponse?> GetByIdAsync(long id);
        Task<List<BranchResponse>> GetAllBranchAsync();
        Task<BranchListResponse> GetAllAsync(int page, int count);
    }
}
