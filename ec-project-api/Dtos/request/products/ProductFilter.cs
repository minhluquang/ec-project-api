namespace ec_project_api.Dtos.request.products {
    public class ProductFilter {
        public string? StatusName { get; set; }
        public string? Search {get; set;}
        public int? ProductGroupId { get; set; }
        public short? CategoryId { get; set; }
        public short? ColorId { get; set; }
        public short? MaterialId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
