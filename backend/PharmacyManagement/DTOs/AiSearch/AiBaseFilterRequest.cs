namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiBaseFilterRequest
    {
        private int _page = 1;
        public int? Page
        {
            get => _page;
            set => _page = (value ?? 1) < 1 ? 1 : value.Value;
        }

        private int _count = 20;
        public int? Count
        {
            get => _count;
            set => _count = (value ?? 20) < 1 ? 20 : (value.Value > 50 ? 50 : value.Value);
        }
    }
}