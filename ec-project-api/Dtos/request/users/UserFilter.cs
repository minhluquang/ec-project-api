namespace ec_project_api.Dtos.request.users
{
    public class UserFilter
    {
        public string? StatusName { get; set; }
        public string? Search { get; set; }
        public string? Phone { get; set; }
        public bool? HasRole { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
