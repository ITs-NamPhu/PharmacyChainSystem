namespace PharmacyManagement.DTOs.AiSearch
{
    public class AiCustomerFilterRequest : AiBaseFilterRequest
    {
        public long? CustomerId { get; set; }
        public string? Keyword { get; set; }
        public long? CustomerTypeId { get; set; }
        public bool? HasActiveDebt { get; set; }
        public decimal? MinWalletBalance { get; set; }
        public decimal? MaxWalletBalance { get; set; }
    }

    public class AiCustomerItem
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? CustomerTypeName { get; set; }
        public decimal WalletBalance { get; set; }
        public decimal DebtAmount { get; set; }
    }
}