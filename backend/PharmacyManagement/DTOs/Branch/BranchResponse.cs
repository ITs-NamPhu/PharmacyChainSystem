namespace PharmacyManagement.DTOs.Branch
{
    public class BranchResponse
    {
        public long BranchID { get; set; }
        public string BranchName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public long PriceListID { get; set; }
    }
}
