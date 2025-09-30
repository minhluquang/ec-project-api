using Azure.Core;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.product_images;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services {

    public class ProductService : BaseService<Product, int>, IProductService {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageService _productImageService;
        private readonly DataContext _dbContext;

        public ProductService(IProductRepository productRepository, IProductImageService productImageService, DataContext dbContext) : base(productRepository) {
            _productRepository = productRepository;
            _productImageService = productImageService;
            _dbContext = dbContext;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync(QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            var products = await _productRepository.GetAllAsync(options);
            return products;
        }

        public override async Task<Product?> GetByIdAsync(int id, QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(v => v.Size)
            );
            options.Includes.Add(p => p.ProductImages);

            var product = await _productRepository.GetByIdAsync(id, options);
            return product;
        }

        public async Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage) {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try {
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                productImage.ProductId = product.ProductId;

                var uploadResult = await _productImageService.UploadSingleProductImageAsync(productImage, FileImage);
                if (!uploadResult) throw new Exception("Upload ảnh thất bại");

                await transaction.CommitAsync();
                return true;
            }
            catch {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}