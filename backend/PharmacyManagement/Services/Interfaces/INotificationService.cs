using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendDebtNotificationAsync(CustomerDebtSummary debt);
        Task SendInvoiceCreatedAsync(Invoice invoice, Customer customer);
    }
}
