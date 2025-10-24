using ec_project_api.Dtos.response.locations;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Dtos.Users;

namespace ec_project_api.DTOs.response.addresses
{
    public class AddressDto
    {
        public int AddressId { get; set; } 
        public string RecipientName { get; set; }= null!;
        public string Phone { get; set; } = null!;
        public string StreetAddress { get; set; }= null!;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
         // Basic refs
         public UserDto? User { get; set; }
         public ProvinceDto? Province { get; set; }
         public WardDto? Ward { get; set; }
    }
}