namespace PharmacyManagement.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermission(long userId);
    }
}
