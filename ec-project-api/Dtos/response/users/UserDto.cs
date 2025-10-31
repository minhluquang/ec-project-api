using ec_project_api.Dtos.response.users;
using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.Users
{
    public class UserDto
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public StatusDto? Status { get; set; }

        public IEnumerable<RoleDto> Roles { get; set; } = new List<RoleDto>(); 
        public IEnumerable<AddressDto> Addresses { get; set; } = new List<AddressDto>(); 
    }
}
