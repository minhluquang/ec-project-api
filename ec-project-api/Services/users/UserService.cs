using ec_project_api.Dtos.response.pagination;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ec_project_api.Constants.Messages;

namespace ec_project_api.Services
{
    public class UserService : BaseService<User, int>, IUserService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _uploadPresent = "unsigned_preset";
        
        public UserService(IUserRepository repository, IConfiguration configuration) : base(repository)
        {
            var account = new Account(
                configuration["Cloudinary:MY_CLOUD_NAME"],
                configuration["Cloudinary:MY_API_KEY"],
                configuration["Cloudinary:MY_API_SECRET"]
            );
            _uploadPresent = configuration["Cloudinary:UPLOAD_PRESET"] ?? "unsigned_preset";
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public override async Task<PagedResult<User>> GetAllPagedAsync(QueryOptions<User>? options = null)
        {
            options ??= new QueryOptions<User>();

            options.Includes.Add(u => u.Status);
            options.Includes.Add(u => u.Addresses);
            options.IncludeThen.Add(q => q
                .Include(u => u.UserRoleDetails)
                    .ThenInclude(urd => urd.Role));

            return await base.GetAllPagedAsync(options);
        }

        public override async Task<User?> GetByIdAsync(int id, QueryOptions<User>? options = null)
        {
            options ??= new QueryOptions<User>();

            options.Includes.Add(u => u.Status);
            options.Includes.Add(u => u.Addresses);
            options.Includes.Add(u => u.UserRoleDetails);

            options.IncludeThen.Add(q => q
                .Include(u => u.UserRoleDetails)
                    .ThenInclude(urd => urd.Role));

            return await _repository.GetByIdAsync(id, options);
        }

        public async Task<bool> UploadAvatarImageAsync(User user, IFormFile fileImage)
        {
            string publicId = $"avatar_{user.UserId}";
            
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileImage.FileName, fileImage.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = false,
                QualityAnalysis = false,
                Folder = "avatars",
                UploadPreset = _uploadPresent,
                PublicId = publicId,
            };
            
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult?.SecureUrl == null)
                throw new Exception(UserMessages.AvatarImageUploadFailed);
            
            user.ImageUrl = uploadResult.SecureUrl.ToString();
            user.UpdatedAt = DateTime.UtcNow;
            
            return await UpdateAsync(user);
        }
        
        public async Task<bool> DeleteAvatarImageAsync(User user)
        {
            string? imageUrl = user.ImageUrl;
            if (string.IsNullOrEmpty((imageUrl)))
                throw new Exception(UserMessages.AvatarImageUploadFailed);         
                    
            int folderIndex = imageUrl.LastIndexOf("avatars/");
            if (folderIndex < 0)
                throw new InvalidOperationException("Invalid avatar URL.");
            
            int dotIndex = imageUrl.LastIndexOf('.');
                if (dotIndex < 0 || dotIndex < folderIndex)
                    throw new InvalidOperationException("Invalid avatar URL format.");
            
                string publicId = imageUrl.Substring(folderIndex, dotIndex - folderIndex);
            
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
            
            if (deleteResult?.Result != "ok")
                throw new InvalidOperationException(UserMessages.AvatarImageDeleteCloudinaryFailed);

            // Delete image record from database
            user.ImageUrl = null;
            return await UpdateAsync(user);
        }
    }
}
