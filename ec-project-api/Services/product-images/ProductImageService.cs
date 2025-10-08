using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.IdentityModel.Tokens;

namespace ec_project_api.Services.product_images {
    public class ProductImageService : BaseService<ProductImage, int>, IProductImageService {
        private readonly IProductImageRepository _productImageRepository;
        private readonly Cloudinary _cloudinary;
        private readonly string _uploadPresent = "unsigned_preset";

        public ProductImageService(IProductImageRepository productImageRepository, IConfiguration configuration) : base(productImageRepository) {
            _productImageRepository = productImageRepository;
            var account = new Account(
                configuration["Cloudinary:MY_CLOUD_NAME"],
                configuration["Cloudinary:MY_API_KEY"],
                configuration["Cloudinary:MY_API_SECRET"]
            );
            _uploadPresent = configuration["Cloudinary:UPLOAD_PRESET"] ?? "unsigned_preset";
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<IEnumerable<ProductImage>> GetAllByProductIdAsync(int productId, QueryOptions<ProductImage>? options = null) {
            options ??= new QueryOptions<ProductImage>();

            options.Filter = pi => pi.ProductId == productId;
            var productImages = await _productImageRepository.GetAllAsync(options);

            return productImages;
        }

        public async Task<bool> UploadSingleProductImageAsync(ProductImage productImage, IFormFile fileImage) {
            // Set display order for the new image
            var productImages = (await GetAllByProductIdAsync(productImage.ProductId)).ToList();

            var lastProductImageDisPlayOrder = productImages.Max(pi => pi.DisplayOrder) ?? 0;
            productImage.DisplayOrder = (byte?)(lastProductImageDisPlayOrder + 1);

            // Insert to get productImageIo for creating publicId Cloudnary
            await CreateAsync(productImage);

            string publicId = $"products/{productImage.ProductId}_{productImage.ProductImageId}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileImage.FileName, fileImage.OpenReadStream()),
                UseFilename = true,
                UniqueFilename = false,
                QualityAnalysis = false,
                UploadPreset = _uploadPresent,
                PublicId = publicId,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult?.SecureUrl == null)
                throw new InvalidOperationException(ProductMessages.ProductImageUploadFailed);

            productImage.ImageUrl = uploadResult.SecureUrl.ToString();

            // Set all images of the product to non-primary if the new image is primary
            if (productImage.IsPrimary) {
                var existingPrimaryImages = productImages.Where(pi => pi.IsPrimary);
                foreach (var pi in existingPrimaryImages) {
                    pi.IsPrimary = false;
                    await UpdateAsync(pi);
                }
            }

            await UpdateAsync(productImage);
            await RefactorDisplayOrderAsync(productImage.ProductId);

            return true;
        }

        public async Task<bool> UpdateImageDisplayOrderAsync(int productId, List<ProductUpdateImageDisplayOrderRequest> request) {
            var productImages = (await GetAllByProductIdAsync(productId)).ToList();

            foreach (var req in request) {
                var productImage = productImages.FirstOrDefault(pi => pi.ProductImageId == req.ProductImageId);
                if (productImage != null) {
                    productImage.DisplayOrder = req.DisplayOrder;
                    productImage.IsPrimary = req.DisplayOrder == 1;
                    productImage.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _productImageRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSingleProductImageAsync(ProductImage productImage) {
            string imageUrl = productImage.ImageUrl;
            if (imageUrl.IsNullOrEmpty())
                throw new InvalidOperationException(ProductMessages.ProductImageNotFound);

            int lastProduct = imageUrl.LastIndexOf("products/");
            int lastDot = imageUrl.LastIndexOf('.');
            string publicId = imageUrl.Substring(lastProduct, lastDot - lastProduct);

            // Delete from Cloudinary
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            // Check if deletion was successful
            if (deleteResult?.Result != "ok")
                throw new InvalidOperationException(ProductMessages.ProductImageDeleteCloudinaryFailed);

            // Delete image record from database
            await DeleteAsync(productImage);
            await RefactorDisplayOrderAsync(productImage.ProductId);

            return true;
        }

        public async Task<ProductImage?> GetNextPrimaryCandidateAsync(int productId, int currentProductImageId) {
            var options = new QueryOptions<ProductImage>();

            options.Filter = pi => pi.ProductId == productId && pi.ProductImageId != currentProductImageId;
            options.OrderBy = query => query.OrderBy(pi => pi.DisplayOrder);

            var productImages = await _productImageRepository.GetAllAsync(options);
            return productImages.FirstOrDefault();
        }

        public async Task<bool> RefactorDisplayOrderAsync(int productId) {
            var productImages = (await GetAllByProductIdAsync(productId))
                .OrderBy(pi => pi.DisplayOrder)
                .ToList();

            if (!productImages.Any())
                return true;

            byte displayOrder = 1;
            foreach (var image in productImages) {
                image.DisplayOrder = displayOrder++;
                image.UpdatedAt = DateTime.UtcNow;
            }

            await _productImageRepository.UpdateRangeAsync(productImages);
            return true;
        }
    }
}