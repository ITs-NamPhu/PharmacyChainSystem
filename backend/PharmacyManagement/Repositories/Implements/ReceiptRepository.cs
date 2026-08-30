using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly PharmacySystemDbContext _context;
        private IDbContextTransaction? _transaction;

        public ReceiptRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCustomerExistsAsync(long customerId)
        {
            return await _context.Customer.AnyAsync(c => c.CustomerID == customerId);
        }

        public async Task<Customer?> GetCustomerAsync(long customerId)
        {
            return await _context.Customer
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task AddAsync(Receipt entity)
        {
            await _context.Receipt.AddAsync(entity);
        }

        public void Update(Receipt entity)
        {
            _context.Receipt.Update(entity);
        }

        public void Delete(Receipt entity)
        {
            _context.Receipt.Remove(entity);
        }

        public async Task<Receipt?> GetByIdAsync(long id)
        {
            return await _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.Branch)
                .Include(r => r.User)
                .Include(r => r.ReceiptDetail)
                    .ThenInclude(d => d!.Invoice)
                .FirstOrDefaultAsync(r => r.ReceiptID == id);
        }

        public IQueryable<Receipt> GetQuery()
        {
            return _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.Branch)
                .Include(r => r.User)
                .AsNoTracking();
        }

        public async Task<List<Receipt>> GetAllAsync(int skip, int take, long? customerId)
        {
            IQueryable<Receipt> query = _context.Receipt
                .Include(r => r.Customer)
                .Include(r => r.Branch)
                .Include(r => r.User);

            if (customerId.HasValue)
                query = query.Where(r => r.CustomerID == customerId.Value);

            return await query
                .OrderByDescending(r => r.CreatedDate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountAsync(long? customerId)
        {
            var query = _context.Receipt.AsQueryable();

            if (customerId.HasValue)
                query = query.Where(r => r.CustomerID == customerId.Value);

            return await query.CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // ===== Chức năng mới: phiếu thu gạch nợ theo FIFO =====

        public async Task<List<Invoice>> GetUnpaidInvoicesFifoAsync(long customerId)
        {
            // Bước 1: Lấy danh sách hóa đơn còn nợ của khách (bất kể chi nhánh nào),
            //         hóa đơn phát sinh nợ trước (CreatedAt cũ nhất) được xếp lên trên.
            return await _context.Invoice
                .Where(i => i.CustomerID == customerId
                            && (i.TotalAmount - i.PaidAmount) > 0)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasDetailsAsync(long receiptId)
        {
            return await _context.ReceiptDetail
                .AnyAsync(d => d.ReceiptID == receiptId);
        }

        public async Task AddDetailsRangeAsync(IEnumerable<ReceiptDetail> details)
        {
            await _context.ReceiptDetail.AddRangeAsync(details);
        }

        public async Task AddWalletHistoryAsync(CustomerWalletHistory history)
        {
            await _context.CustomerWalletHistory.AddAsync(history);
        }

        public async Task UpdateCustomerWalletAsync(long customerId, decimal delta)
        {
            var query = _context.Customer.Where(c => c.CustomerID == customerId);

            // Nếu là trừ tiền thì chỉ trừ khi số dư đủ lớn (tránh ví bị âm)
            if (delta < 0)
                query = query.Where(c => c.WalletBalance >= -delta);

            // Cập nhật atomic ngay trong database để tránh ghi đè khi có nhiều yêu cầu cùng lúc
            await query.ExecuteUpdateAsync(s => s.SetProperty(c => c.WalletBalance, c => c.WalletBalance + delta));
        }

        public async Task BeginTransactionAsync()
        {
            // Serializable: khóa vùng đọc để 2 phiếu thu của cùng 1 khách không gạch trùng hóa đơn
            _transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null) return;
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}