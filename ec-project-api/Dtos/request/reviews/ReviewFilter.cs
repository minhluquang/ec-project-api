namespace ec_project_api.Dtos.request.reviews {
    public class ReviewFilter {
        public string? StatusName { get; set; }
        public string? Search { get; set; }
        public string? Rating { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Username { get; set; }
    }
}