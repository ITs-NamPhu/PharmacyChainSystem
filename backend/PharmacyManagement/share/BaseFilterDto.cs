namespace PharmacyManagement.share
{
    public class BaseFilterDto
    {
        public string? Keyword { get; set; }

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 20 : (value > 100 ? 100 : value);
        }

        public string? SortBy { get; set; }

        public bool IsDescending { get; set; } = true;
    }
}
