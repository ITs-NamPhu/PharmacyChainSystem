using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.share;
using System.Drawing;

namespace PharmacyManagement.Models
{
    public class PharmacySystemDbContext : DbContext
    {
        public PharmacySystemDbContext(DbContextOptions<PharmacySystemDbContext> options) : base(options) { }

        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<WareHouse> WareHouse { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<UserBranch> UserBranch { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }

        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerType> CustomerType { get; set; }
        public DbSet<CustomerDebtSummary> CustomerDebtSummary { get; set; }

        public DbSet<ManuFacturer> ManuFacturer { get; set; }
        public DbSet<MedicineCategory> MedicineCategory { get; set; }
        public DbSet<Unit> Unit { get; set; }
        public DbSet<UnitConversion> UnitConversion { get; set; }
        public DbSet<Medicine> Medicine { get; set; }
        public DbSet<Batch> Batch { get; set; }

        public DbSet<PriceList> PriceList { get; set; }
        public DbSet<PriceListItem> PriceListItem { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<PromotionItem> PromotionItem { get; set; }


        public DbSet<GoodsReceipt> GoodsReceipt { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItem { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturn { get; set; }
        public DbSet<PurchaseReturnItem> PurchaseReturnItem { get; set; }
        public DbSet<SalesReturn> SalesReturn { get; set; }
        public DbSet<SalesReturnItem> SalesReturnItem { get; set; }
        public DbSet<Receipt> Receipt { get; set; }

        public DbSet<DestroyReceipt> DestroyReceipt { get; set; }
        public DbSet<DestroyReceiptItem> DestroyReceiptItem { get; set; }
        public DbSet<StockTake> StockTake { get; set; }
        public DbSet<StockTakeItem> StockTakeItem { get; set; }
        public DbSet<StockAdjustment> StockAdjustment { get; set; }
        public DbSet<StockAdjustmentItem> StockAdjustmentItem { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<ChatConversation> ChatConversation { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Branch>().ToTable("Branch");
            modelBuilder.Entity<WareHouse>().ToTable("WareHouse");
            modelBuilder.Entity<Supplier>().ToTable("Supplier");
            modelBuilder.Entity<UserBranch>().ToTable("UserBranch");
            modelBuilder.Entity<Permission>().ToTable("Permission");
            modelBuilder.Entity<RolePermission>().ToTable("RolePermission");

            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<CustomerType>().ToTable("CustomerType");
            modelBuilder.Entity<CustomerDebtSummary>().ToTable("CustomerDebtSummary");

            modelBuilder.Entity<ManuFacturer>().ToTable("ManuFacturer");
            modelBuilder.Entity<MedicineCategory>().ToTable("MedicineCategory");
            modelBuilder.Entity<Unit>().ToTable("Unit");
            modelBuilder.Entity<UnitConversion>().ToTable("UnitConversion");
            modelBuilder.Entity<Medicine>().ToTable("Medicine");
            modelBuilder.Entity<Batch>().ToTable("Batch");

            modelBuilder.Entity<PriceList>().ToTable("PriceList");
            modelBuilder.Entity<PriceListItem>().ToTable("PriceListItem");
            modelBuilder.Entity<Promotion>().ToTable("Promotion");
            modelBuilder.Entity<PromotionItem>().ToTable("PromotionItem");

            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<InvoiceItem>().ToTable("InvoiceItem");
            modelBuilder.Entity<GoodsReceipt>().ToTable("GoodsReceipt");
            modelBuilder.Entity<GoodsReceiptItem>().ToTable("GoodsReceiptItem");
            modelBuilder.Entity<PurchaseReturn>().ToTable("PurchaseReturn");
            modelBuilder.Entity<PurchaseReturnItem>().ToTable("PurchaseReturnItem");
            modelBuilder.Entity<SalesReturn>().ToTable("SalesReturn");
            modelBuilder.Entity<SalesReturnItem>().ToTable("SalesReturnItem");
            modelBuilder.Entity<Receipt>().ToTable("Receipt");

            modelBuilder.Entity<DestroyReceipt>().ToTable("DestroyReceipt");
            modelBuilder.Entity<DestroyReceiptItem>().ToTable("DestroyReceiptItem");
            modelBuilder.Entity<StockTake>().ToTable("StockTake");
            modelBuilder.Entity<StockTakeItem>().ToTable("StockTakeItem");
            modelBuilder.Entity<StockAdjustment>().ToTable("StockAdjustment");
            modelBuilder.Entity<StockAdjustmentItem>().ToTable("StockAdjustmentItem");
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");

            modelBuilder.Entity<ChatConversation>().ToTable("ChatConversation");
            modelBuilder.Entity<ChatMessage>().ToTable("ChatMessage");

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.RolePermissionID);
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RolePermission)
                    .HasForeignKey(e => e.RoleID);
                entity.HasOne(e => e.Permission)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(e => e.PermissionID);
            });

            modelBuilder.Entity<UserBranch>(entity =>
            {
                entity.HasOne(e => e.Role)
                    .WithMany(r => r.UserBranch)
                    .HasForeignKey(e => e.RoleID);
            });

            modelBuilder.Entity<StockAdjustmentItem>(entity =>
            {
                entity.HasOne(e => e.StockTakeItem)
                    .WithMany()
                    .HasForeignKey(e => e.StockTakeItemID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DestroyReceipt>(entity =>
            {
                entity.HasOne(e => e.StockTake)
                    .WithMany()
                    .HasForeignKey(e => e.StockTakeID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DestroyReceiptItem>(entity =>
            {
                entity.HasOne(e => e.StockTakeItem)
                    .WithMany()
                    .HasForeignKey(e => e.StockTakeItemID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatConversation>(entity =>
            {
                entity.HasKey(e => e.ConversationID);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageID);

                entity.HasOne(e => e.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(e => e.ConversationID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => new { i.BranchID, i.CreatedAt })
                .HasDatabaseName("IX_Invoice_BranchID_CreatedAt");

            modelBuilder.Entity<GoodsReceipt>()
                .HasIndex(gr => new { gr.BranchID, gr.ReceiptDate })
                .HasDatabaseName("IX_GoodsReceipt_BranchID_ReceiptDate");

            modelBuilder.Entity<Batch>()
                .HasIndex(b => b.ExpiryDate)
                .HasDatabaseName("IX_Batch_ExpiryDate");

            // Global Query Filter: loại bỏ các bản ghi đã bị xóa mềm (IsDeleted = true)
            modelBuilder.Entity<Customer>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<ManuFacturer>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Medicine>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Supplier>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Invoice>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<GoodsReceipt>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Receipt>().HasQueryFilter(p => !p.IsDeleted);

        }

        public override int SaveChanges()
        {
            HandleSoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void HandleSoftDelete()
        {
            // Lấy tất cả các object đang bị đánh dấu là "Deleted" (chuẩn bị xóa)
            var deletedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDelete);

            foreach (var entry in deletedEntries)
            {
                // 1. Đổi trạng thái từ Deleted sang Modified để EF Core sinh lệnh UPDATE thay vì DELETE
                entry.State = EntityState.Modified;

                // 2. Ép kiểu entity về ISoftDelete và gán IsDeleted = true
                var entity = (ISoftDelete)entry.Entity;
                entity.IsDeleted = true;
            }
        }
    }
}

