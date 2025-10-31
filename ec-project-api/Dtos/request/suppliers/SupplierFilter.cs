namespace ec_project_api.Dtos.request.suppliers
{
    public class SupplierFilter
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

        public int? StatusId { get; set; }
        public string? Name { get; set; }
        public string? OrderBy { get; set; }
    }
}
public class SupplierFilter
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int? StatusId { get; set; }
    public string? Name { get; set; }
    public string? OrderBy { get; set; }
}
