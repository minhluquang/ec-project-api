namespace ec_project_api.Dtos.request.products {
    public class ProductCategorySlugFilter {
        public string? Search {get; set;}
        public string? CategorySlug {get;set;}
        public List<short>? ColorIds { get; set; }
        public List<short>? MaterialIds { get; set; }
        public List<int>? ProductGroupIds { get; set; }
        public string? OrderBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? OutOfStock { get; set; }
        public bool? InStock { get; set; }  
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}