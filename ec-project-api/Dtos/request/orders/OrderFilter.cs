namespace ec_project_api.Dtos.request.orders
{
    public class OrderFilter
    {
        public string? StatusName { get; set; }
        public string? Search { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
