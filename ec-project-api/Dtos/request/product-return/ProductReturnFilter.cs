namespace ec_project_api.Dtos.request.product_return
{
    public class ProductReturnFilter
    {
        public string? StatusName { get; set; }
        public string? Search { get; set; }
        public int? ReturnType { get; set; } 
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
