using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IReceiptRepository
    {
        Task<bool> IsCustomerExistsAsync(long customerId);

        // Lấy khách hàng (kèm số dư ví) để phục vụ nạp tiền thừa vào ví
        Task<Customer?> GetCustomerAsync(long customerId);

        Task AddAsync(Receipt entity);
        void Update(Receipt entity);
        void Delete(Receipt entity);
        Task<Receipt?> GetByIdAsync(long id);
        IQueryable<Receipt> GetQuery();
        Task<List<Receipt>> GetAllAsync(int skip, int take, long? customerId);
        Task<int> CountAsync(long? customerId);
        Task SaveChangesAsync();

        // ===== Chức năng mới: phiếu thu gạch nợ theo FIFO =====

        // BƯỚC 1 của thuật toán FIFO:
        // Lấy các hóa đơn còn nợ của khách, hóa đơn cũ nhất đứng trước
        Task<List<Invoice>> GetUnpaidInvoicesFifoAsync(long customerId);

        // Kiểm tra phiếu thu đã có dòng gạch nợ hay chưa (dùng để chặn sửa/xóa)
        Task<bool> HasDetailsAsync(long receiptId);

        Task AddDetailsRangeAsync(IEnumerable<ReceiptDetail> details);
        Task AddWalletHistoryAsync(CustomerWalletHistory history);

        // Cộng / trừ số dư ví của khách hàng (atomic, tránh sai khi chạy đồng thời)
        Task UpdateCustomerWalletAsync(long customerId, decimal delta);

        // Transaction cho toàn bộ quy trình tạo phiếu thu (BƯỚC 3)
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}