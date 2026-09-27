namespace PharmacyManagement.Services.BatchSelection
{
    /// <summary>
    /// Phân tích xu hướng bán hàng theo thời gian bằng hồi quy tuyến tính đơn giản.
    ///
    /// Ý tưởng: vẽ đường thẳng "vừa vặn nhất" đi qua chuỗi doanh số từng ngày,
    /// độ dốc (slope) của đường thẳng cho biết xu hướng tăng hay giảm:
    ///     m = [n * Sum(xy) - Sum(x) * Sum(y)] / [n * Sum(x^2) - (Sum(x))^2]
    ///
    /// Yêu cầu đầu vào:
    /// - Đã điền 0 cho các ngày không có giao dịch (nếu bỏ trống, hồi quy sẽ bị lệch).
    /// - Đúng thứ tự thời gian từ CŨ đến MỚI, vì x chính là chỉ số thứ tự ngày.
    /// </summary>
    public static class SalesTrendAnalyzer
    {
        public const string TrendIncreasing = "INCREASING";
        public const string TrendDecreasing = "DECREASING";
        public const string TrendStable = "STABLE";

        /// <summary>
        /// Ngưỡng dao động tối thiểu, tránh báo "Tăng/Giảm" chỉ vì doanh số
        /// nhiễu 1-2 sản phẩm trên mặt hàng bán chậm.
        /// </summary>
        public const double MinTrendThreshold = 0.1;

        /// <summary>
        /// Ngưỡng dao động tương đối: bằng 5% bình quân ngày.
        /// Mặt hàng bán nhanh (hàng trăm/ngày) sẽ có ngưỡng cao hơn, tránh báo nhiễu.
        /// </summary>
        public const double TrendRatioOfAverage = 0.05;

        /// <summary>
        /// Số ngày dùng để đo "đà tăng/giảm gần đây".
        /// Đường hồi quy trên cả 30 ngày phản ứng quá chậm với cú tăng 2-3 ngày cuối,
        /// nên cần cửa sổ ngắn để bắt tín hiệu mà quản lý kho quan tâm nhất.
        /// </summary>
        public const int MomentumWindowDays = 7;

        /// <summary>
        /// Cửa sổ ngắn tối thiểu để đủ tin cậy, nếu ít hơn thì bỏ qua đà gần đây.
        /// </summary>
        private const int MinMomentumWindow = 4;

        /// <summary>
        /// Tính xu hướng bán hàng từ chuỗi doanh số theo ngày (đã điền 0 cho ngày trống).
        /// </summary>
        /// <param name="dailySales">Doanh số từng ngày, thứ tự từ CŨ đến MỚI.</param>
        /// <returns>"INCREASING" | "DECREASING" | "STABLE"</returns>
        public static string CalculateTrend(IReadOnlyList<decimal> dailySales)
        {
            // Cần ít nhất 2 điểm mới vẽ được đường thẳng
            if (dailySales == null || dailySales.Count < 2)
                return TrendStable;

            int n = dailySales.Count;
            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumX2 = 0;

            for (int i = 0; i < n; i++)
            {
                double x = i + 1;             // Trục X: chỉ số thứ tự ngày (1 đến n)
                double y = (double)dailySales[i]; // Trục Y: số lượng bán trong ngày

                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominator = (n * sumX2) - (sumX * sumX);

            // Mẫu toàn bằng 0 hoặc trục X không hợp lệ -> không có xu hướng
            if (denominator == 0 || sumY == 0)
                return TrendStable;

            double slope = ((n * sumXY) - (sumX * sumY)) / denominator;
            double threshold = ResolveThreshold(sumY / n);

            if (slope > threshold)
                return TrendIncreasing;
            if (slope < -threshold)
                return TrendDecreasing;

            return TrendStable;
        }

        /// <summary>
        /// Ngưỡng dao động động: max(0.1, 5% x bình quân ngày).
        /// </summary>
        public static double ResolveThreshold(double averageDaily)
        {
            return Math.Max(MinTrendThreshold, Math.Abs(averageDaily) * TrendRatioOfAverage);
        }

        /// <summary>
        /// Xác định xu hướng cuối cùng bằng cách kết hợp hai cửa sổ:
        /// toàn bộ kỳ và MomentumWindowDays ngày cuối.
        ///
        /// Nếu đà gần đây rõ ràng thì lấy đà gần đây, vì đó mới là tín hiệu
        /// quyết định nên nhập hàng tuần tới. Ví dụ 27 ngày đều 4-5 hộp rồi
        /// 3 ngày cuối tăng vọt: hồi quy toàn kỳ ra STABLE, nhưng thực tế là TĂNG.
        /// Ngược lại, khi cả hai cùng STABLE thì lấy kết luận toàn kỳ.
        /// </summary>
        public static string ResolveTrend(IReadOnlyList<decimal> dailySales)
        {
            string longTrend = CalculateTrend(dailySales);

            if (dailySales == null || dailySales.Count < MinMomentumWindow)
                return longTrend;

            int window = Math.Min(MomentumWindowDays, dailySales.Count);
            var recent = new List<decimal>(window);
            for (int i = dailySales.Count - window; i < dailySales.Count; i++)
            {
                recent.Add(dailySales[i]);
            }

            string momentumTrend = CalculateTrend(recent);
            return momentumTrend != TrendStable ? momentumTrend : longTrend;
        }
    }
}
