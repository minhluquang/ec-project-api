using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface IUserService : IBaseService<User, int>
    {
        Task<bool> UploadAvatarImageAsync(User user, IFormFile fileImage);
        Task<bool> DeleteAvatarImageAsync(User user);
    }
}