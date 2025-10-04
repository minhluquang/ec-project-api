namespace ec_project_api.Dtos.request.suppliers
{
    public class SupplierQueryRequest
    {
        public string? Search { get; set; }          
        public int? Status { get; set; }            
        public int Page { get; set; } = 1;         
        public int PageSize { get; set; } = 10;   
    }
}
