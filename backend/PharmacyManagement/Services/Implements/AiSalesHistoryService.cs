using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.AiSearch;
using PharmacyManagement.Services.BatchSelection;

namespace PharmacyManagement.Services.Implements
{
    public partial class AiSearchService
    {
        /// <summary>
        /// Lịch sử bán hàng theo từng ngày của một mặt hàng tại một chi nhánh,
        /// kèm tổng hợp và xu hướng đã tính sẵn để AI không phải tự tính toán.
        ///
        /// Số lượng lấy trên InvoiceItem.BaseQuantity (đã quy đổi về đơn vị cơ sở)
        /// để so sánh được giữa các mặt hàng khác nhau.
        /// Không trừ SalesReturnItem vì phiếu trả hàng gắn với từng lô riêng lẻ.
        /// </summary>
        public async Task<AiSalesHistoryDto> GetSalesHistoryAsync(
            AiSalesHistoryRequest filter, long? branchScope)
        {
            long? branchId = branchScope ?? filter.BranchId;
            int days = filter.Days < 1 ? 30 : filter.Days;
            var today = DateTime.Today;
            var from = today.AddDays(-(days - 1));
            var toExclusive = today.AddDays(1);

            var (medicineId, medicineName) = await ResolveMedicineAsync(filter);

            var query = _context.InvoiceItem
                .AsNoTracking()
                .Where(ii => ii.Invoice != null
                    && ii.Batch != null
                    && ii.Batch.GoodsReceiptItem != null
                    && ii.Batch.GoodsReceiptItem.MedicineID == medicineId);

            if (branchId.HasValue)
                query = query.Where(ii => ii.Invoice!.BranchID == branchId.Value);

            // So sánh trực tiếp để không bỏ sót hóa đơn phát sinh sau nửa đêm
            query = query.Where(ii => ii.Invoice!.CreatedAt >= from
                && ii.Invoice!.CreatedAt < toExclusive);

            var rows = await query
                .GroupBy(ii => ii.Invoice!.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Quantity = g.Sum(ii => ii.BaseQuantity)
                })
                .ToListAsync();

            var byDate = rows.ToDictionary(x => x.Date, x => x.Quantity);

            // Điền 0 cho ngày không bán: bắt buộc để hồi quy tuyến tính không bị lệch,
            // và để AverageDaily chia đúng theo số ngày trong cửa sổ.
            var dailyRecords = new Dictionary<string, decimal>();
            var orderedSales = new List<decimal>(days);
            decimal total = 0m;

            for (int offset = 0; offset < days; offset++)
            {
                var day = from.AddDays(offset);
                var quantity = byDate.TryGetValue(day, out var value) ? value : 0m;

                dailyRecords[day.ToString("yyyy-MM-dd")] = quantity;
                orderedSales.Add(quantity);
                total += quantity;
            }

            decimal averageDaily = days > 0 ? Math.Round(total / days, 2) : 0m;

            return new AiSalesHistoryDto
            {
                ProductId = medicineId,
                ProductName = medicineName,
                BranchId = branchId,
                PeriodDays = days,
                UnitName = await GetBaseUnitNameAsync(medicineId),
                Summary = new SalesSummaryDto
                {
                    TotalQuantitySold = total,
                    AverageDaily = averageDaily,
                    SalesTrend = SalesTrendAnalyzer.ResolveTrend(orderedSales)
                },
                DailyRecords = dailyRecords
            };
        }

        /// <summary>
        /// Xác định thuốc cần tra cứu. Ưu tiên MedicineId, sau đó mới tới Keyword.
        /// Keyword khớp nhiều thuốc -> báo lỗi để AI hỏi lại người dùng thay vì đoán.
        /// </summary>
        private async Task<(long MedicineId, string? MedicineName)> ResolveMedicineAsync(
            AiSalesHistoryRequest filter)
        {
            if (filter.MedicineId.HasValue)
            {
                long medicineId = filter.MedicineId.Value;
                var name = await _context.Medicine
                    .AsNoTracking()
                    .Where(m => m.MedicineID == medicineId)
                    .Select(m => m.MedicineName)
                    .FirstOrDefaultAsync();

                if (name == null)
                    throw new Exceptions.BusinessException(
                        $"Khong tim thay thuoc co id = {medicineId}.",
                        "HIS003");

                return (medicineId, name);
            }

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();
                var candidates = await _context.Medicine
                    .AsNoTracking()
                    .Where(m => m.MedicineName.Contains(keyword))
                    .OrderBy(m => m.MedicineName)
                    .Select(m => new { m.MedicineID, m.MedicineName })
                    .ToListAsync();

                if (candidates.Count == 0)
                    throw new Exceptions.BusinessException(
                        $"Khong tim thay thuoc nao khop tu khoa '{keyword}'.",
                        "HIS003");

                if (candidates.Count > 1)
                    throw new Exceptions.BusinessException(
                        $"Tu khoa '{keyword}' khop nhieu thuoc: "
                            + string.Join(", ", candidates.Take(8).Select(c => c.MedicineName))
                            + ". Hay chi dinh ten thuoc chinh xac hon.",
                        "HIS002");

                return (candidates[0].MedicineID, candidates[0].MedicineName);
            }

            throw new Exceptions.BusinessException(
                "Can cung cap MedicineId hoac Keyword de tra cuu lich su ban hang.",
                "HIS001");
        }

        /// <summary>
        /// Lấy tên đơn vị cơ sở của thuốc (khớp với BaseQuantity).
        /// Cố ý không dùng navigation Medicine.Unit vì đó là shadow FK chưa được populate.
        /// </summary>
        private async Task<string?> GetBaseUnitNameAsync(long medicineId)
        {
            var unitId = await _context.GoodsReceiptItem
                .AsNoTracking()
                .Where(g => g.MedicineID == medicineId)
                .OrderByDescending(g => g.GoodsReceiptItemID)
                .Select(g => (long?)g.UnitID)
                .FirstOrDefaultAsync();

            if (!unitId.HasValue)
                return null;

            return await _context.Unit
                .AsNoTracking()
                .Where(u => u.UnitID == unitId.Value)
                .Select(u => u.UnitName)
                .FirstOrDefaultAsync();
        }
    }
}
