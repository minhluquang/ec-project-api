using ec_project_api.Interfaces;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.product_images;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ec_project_api.Services {

    public class ProductService : BaseService<Product, int>, IProductService {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageService _productImageService;

        public ProductService(
            IProductRepository productRepository,
            IProductImageService productImageService
        ) : base(productRepository) {
            _productRepository = productRepository;
            _productImageService = productImageService;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync(QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            var products = await _productRepository.GetAllAsync(options);
            return products;
        }

        public override async Task<Product?> GetByIdAsync(int id, QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size));
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Status));
            options.Includes.Add(p => p.ProductImages);

            var product = await _productRepository.GetByIdAsync(id, options);
            return product;
        }

        public async Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage) {
            await base.CreateAsync(product);

            productImage.ProductId = product.ProductId;

            var uploadResult = await _productImageService.UploadSingleProductImageAsync(productImage, FileImage);
            if (!uploadResult)
                throw new Exception("Upload ảnh thất bại");

            return true;
        }
        public async Task<IEnumerable<Product>> GetAllByCategoryidAsync(short categoryId, int? pageNumber = 1, int? pageSize = 12, decimal? minPrice = null, decimal? maxPrice = null, short? colorId = null, string? orderBy = null) {
            var options = new QueryOptions<Product>();

            options.Filter = p => p.CategoryId == categoryId &&
               (!minPrice.HasValue ||
                   (p.DiscountPercentage.HasValue
                       ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                       : p.BasePrice) >= minPrice.Value) &&
               (!maxPrice.HasValue ||
                   (p.DiscountPercentage.HasValue
                       ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                       : p.BasePrice) <= maxPrice.Value) &&
               (!colorId.HasValue || p.ColorId == colorId);

            if (!orderBy.IsNullOrEmpty()) {
                switch (orderBy) {
                    case "name_asc":
                        options.OrderBy = q => q.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.Name);
                        break;
                    case "price_asc":
                        options.OrderBy = q => q.OrderBy(p => p.DiscountPercentage.HasValue ? p.BasePrice - (p.BasePrice * p.DiscountPercentage / 100) : p.BasePrice);
                        break;
                    case "price_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.DiscountPercentage.HasValue ? p.BasePrice - (p.BasePrice * p.DiscountPercentage / 100) : p.BasePrice);
                        break;
                    case "date_old_new":
                        options.OrderBy = q => q.OrderBy(p => p.CreatedAt);
                        break;
                    case "date_new_old":
                        options.OrderBy = q => q.OrderByDescending(p => p.CreatedAt);
                        break;
                }
            }

            options.PageNumber = pageNumber;
            options.PageSize = pageSize;

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            return await _productRepository.GetAllAsync(options);
        }

        public override async Task<bool> DeleteAsync(Product product) {
            var productImages = await _productImageService.GetAllByProductIdAsync(product.ProductId);
            foreach (var image in productImages) {
                await _productImageService.DeleteSingleProductImageAsync(image);
            }
            return await base.DeleteAsync(product);
        }
    }
}