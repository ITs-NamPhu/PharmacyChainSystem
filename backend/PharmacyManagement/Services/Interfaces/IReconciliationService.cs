using PharmacyManagement.Models;

namespace PharmacyManagement.Services.Interfaces
{
    // Kiểm tra định kỳ (2h sáng) xem số tiền đã trả của hóa đơn
    // có khớp với tổng số tiền gạch nợ trong ReceiptDetail hay không
    public interface IReconciliationService
    {
        Task ReconcileAsync();
    }
}