namespace ec_project_api.Dtos.request.products
{
    public class MaterialFilter
    {
        public string? StatusName { get; set; }
        public string? Search { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
