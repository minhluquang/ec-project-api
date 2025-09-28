using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

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

        public async Task<bool> UploadSingleProductImageAsync(ProductImage productImage, IFormFile fileImage, bool findDisplayOrder = true) {
            try {
                // Set display order for the new image
                string publicId;
                var productImages = (await GetAllByProductIdAsync(productImage.ProductId)).ToList();

                if (findDisplayOrder) {
                    var lastProductImageDisPlayOrder = productImages.Max(pi => pi.DisplayOrder) ?? 0;
                    productImage.DisplayOrder = (byte?)(lastProductImageDisPlayOrder + 1);

                    publicId = $"products/{productImage.ProductId}_{productImage.DisplayOrder}";
                }
                else {
                    publicId = $"products/{productImage.ProductId}_{productImage.DisplayOrder}";
                }

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
                if (uploadResult?.SecureUrl == null) return false;

                productImage.ImageUrl = uploadResult.SecureUrl.ToString();

                // Set all images of the product to non-primary if the new image is primary
                if (productImage.IsPrimary == true) {
                    foreach (var pi in productImages.Where(p => p.IsPrimary && productImage.IsPrimary)) {
                        pi.IsPrimary = false;
                        await _productImageRepository.UpdateAsync(pi);
                    }
                }
                await _productImageRepository.AddAsync(productImage);
                await _productImageRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}