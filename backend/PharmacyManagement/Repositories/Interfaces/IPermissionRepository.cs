namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<string>> GetPermissionsByUserIdAsync(long userId);
    }
}
