namespace ec_project_api.DTO
{
    public class CategoryDTO
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
        public string slug { get; set; } = null!;
        public string description { get; set; }
    }
}
