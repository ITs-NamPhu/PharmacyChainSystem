using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendDebtNotificationAsync(CustomerDebtSummary debt);
    }
}
