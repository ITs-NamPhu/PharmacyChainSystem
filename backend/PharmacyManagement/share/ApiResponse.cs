namespace PharmacyManagement.share
{
    public class ApiResponse<T>
    {
        public int EC { get; set; }
        public int StatusCode { get; set; }

        public string EM { get; set; } = string.Empty;

        public T? DT { get; set; }
    }
}
