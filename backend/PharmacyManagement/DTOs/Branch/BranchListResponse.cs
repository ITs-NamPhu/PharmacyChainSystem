namespace PharmacyManagement.DTOs.Branch
{
    public class BranchListResponse
    {
        public int NumRecords { get; set; }
        public float TotalPage { get; set; }
        public List<BranchResponse> Branches { get; set; } = new();
    }
}
