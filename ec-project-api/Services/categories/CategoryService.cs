using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.categories
{
    public class CategoryService : BaseService<Category, short>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly Cloudinary _cloudinary;
        private readonly string _uploadPreset = "unsigned_preset";

        public CategoryService(ICategoryRepository repository, IConfiguration configuration)
            : base(repository)
        {
            _categoryRepository = repository;

            var account = new Account(
                configuration["Cloudinary:MY_CLOUD_NAME"],
                configuration["Cloudinary:MY_API_KEY"],
                configuration["Cloudinary:MY_API_SECRET"]
            );

            _uploadPreset = configuration["Cloudinary:UPLOAD_PRESET"] ?? "unsigned_preset";
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        // ---------------- CATEGORY CRUD ----------------
        public async Task<IEnumerable<Category>> GetByParentIdsAsync(List<short> categoryIds)
        {
            var options = new QueryOptions<Category>();
            var allCategories = await _categoryRepository.GetAllAsync(options);

            return allCategories
                .Where(c => categoryIds.Contains(c.CategoryId) || (c.ParentId.HasValue && categoryIds.Contains(c.ParentId.Value)))
                .ToList();
        }

        public async Task<IEnumerable<Category>> GetByParentIdAsync(short parentId)
        {
            return await FindAsync(c => c.ParentId == parentId);
        }

        public async Task<bool> CreateAsync(Category category, IFormFile? file = null)
        {
            bool created = await base.CreateAsync(category);
            if (!created) throw new InvalidOperationException("Tạo danh mục thất bại.");

            if (file != null && file.Length > 0)
                await UploadAsync(category, file);

            return true;
        }

        public async Task<bool> UpdateAsync(Category category, bool removeImage, IFormFile? file = null)
        {
            if (category == null)
                throw new InvalidOperationException("Không tìm thấy danh mục.");

            var tracked = await base.GetByIdAsync(category.CategoryId)
                ?? throw new InvalidOperationException("Không tìm thấy danh mục trong hệ thống.");

            // Xác định danh mục gốc dựa trên dữ liệu từ DB (tracked)
            bool isRootCategory = tracked.CategoryId is 1 or 2;

            try
            {
                // 1️⃣ Upload ảnh mới nếu có
                if (file != null && file.Length > 0)
                {
                    await UploadAsync(tracked, file);
                }
                else if (removeImage && !string.IsNullOrEmpty(tracked.SizeDetail))
                {
                    await DeleteImageAsync(tracked);
                }

                // 2️⃣ Cập nhật các trường cơ bản (tên/slug/description luôn cho chỉnh)
                tracked.Name = category.Name;
                tracked.Slug = category.Slug;
                tracked.Description = category.Description;

                // 3️⃣ Xử lý ParentId và StatusId theo rules
                if (isRootCategory)
                {
                    // Không cho thay đổi ParentId hoặc Status cho danh mục gốc
                    if (category.ParentId != tracked.ParentId || category.StatusId != tracked.StatusId)
                        throw new InvalidOperationException("Không được thay đổi danh mục cha hoặc trạng thái của danh mục gốc.");
                }
                else
                {
                   
                        tracked.ParentId = category.ParentId;
                  
                        tracked.StatusId = category.StatusId;
                   
                }

                tracked.UpdatedAt = DateTime.UtcNow;
                return await base.UpdateAsync(tracked);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cập nhật danh mục thất bại: {ex.Message}");
            }
        }



        public override async Task<bool> DeleteAsync(Category entity)
        {
            if (entity == null || entity.CategoryId is 1 or 2) return false;

            var tracked = await base.GetByIdAsync(entity.CategoryId) ?? entity;

            if (!string.IsNullOrEmpty(tracked.SizeDetail))
            {
                try
                {
                    await DeleteImageAsync(tracked);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Lỗi khi xoá ảnh Cloudinary: {ex.Message}");
                    return false; // abort DB delete
                }
            }

            return await base.DeleteAsync(tracked);
        }

        public override async Task<bool> DeleteByIdAsync(short id)
        {
            if (id is 1 or 2) return false;

            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            return await DeleteAsync(entity);
        }

        // ---------------- UPLOAD ẢNH ----------------
        public async Task<string> UploadAsync(Category category, IFormFile file)
        {
            if (category == null) throw new InvalidOperationException("Không tìm thấy danh mục.");
            if (file == null || file.Length == 0) throw new InvalidOperationException("Không có file ảnh hợp lệ.");
            if (file.Length > 2 * 1024 * 1024) throw new InvalidOperationException("Kích thước ảnh vượt quá 2MB.");
            if (!file.ContentType.StartsWith("image/")) throw new InvalidOperationException("Định dạng file không hợp lệ.");

            string publicId = $"category_{category.CategoryId}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = false,
                Folder = "categories",
                UploadPreset = _uploadPreset,
                PublicId = publicId,
                Overwrite = true,
                Invalidate = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult?.SecureUrl == null) throw new InvalidOperationException("Tải ảnh danh mục thất bại.");

            var tracked = await base.GetByIdAsync(category.CategoryId) ?? category;
            tracked.SizeDetail = uploadResult.SecureUrl.ToString();
            tracked.UpdatedAt = DateTime.UtcNow;
            await base.UpdateAsync(tracked);

            return tracked.SizeDetail!;
        }

        // ---------------- DELETE ẢNH ----------------
        public async Task<bool> DeleteImageAsync(Category category)
        {
            if (category == null || string.IsNullOrEmpty(category.SizeDetail))
                return false;

            string publicId = ExtractPublicId(category.SizeDetail, "categories");

            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image,
                Invalidate = true
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result?.Result != "ok" && result?.Result != "not_found")
                throw new InvalidOperationException($"Xóa ảnh thất bại: {result?.Result}");

            // Update DB
            category.SizeDetail = null;
            category.UpdatedAt = DateTime.UtcNow;
            await base.UpdateAsync(category);

            return true;
        }

        // ---------------- HELPER ----------------
        private string ExtractPublicId(string imageUrl, string folder)
        {
            if (string.IsNullOrEmpty(imageUrl))
                throw new InvalidOperationException("URL ảnh trống.");

            int folderIndex = imageUrl.IndexOf($"{folder}/", StringComparison.OrdinalIgnoreCase);
            if (folderIndex < 0)
                throw new InvalidOperationException("URL ảnh không chứa folder hợp lệ.");

            string publicIdWithExt = imageUrl.Substring(folderIndex); // categories/category_41.jpg
            int dotIndex = publicIdWithExt.LastIndexOf('.');
            string publicId = dotIndex > 0 ? publicIdWithExt.Substring(0, dotIndex) : publicIdWithExt;

            return publicId.TrimStart('/'); // categories/category_41
        }
    }
}
