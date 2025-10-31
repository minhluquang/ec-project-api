using ec_project_api.Dtos.response.locations;

namespace ec_project_api.Dtos.Users
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public ProvinceDto Province { get; set; } 
        public WardDto Ward { get; set; } 
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
