namespace PharmacyManagement.DTOs.Branch
{
    public class CreateBranchRequest
    {
        public string BranchName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public long PriceListID { get; set; }
    }

    public class UpdateBranchRequest
    {
        public string BranchName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public bool IsActive { get; set; }
        public long? PriceListID { get; set; }
    }
}
