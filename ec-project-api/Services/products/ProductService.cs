using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public class ProductService : BaseService<Product, int>, IProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) : base(productRepository) {
            _productRepository = productRepository;
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
            options.Includes.Add(p => p.ProductVariants);
            options.Includes.Add(p => p.ProductImages);

            options.IncludePaths.Add("ProductVariants.Color");
            options.IncludePaths.Add("ProductVariants.Size");

            var product = await _productRepository.GetByIdAsync(id, options);
            return product;
        }

        public override async Task<bool> CreateAsync(Product product) {
            try {
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                return true;
            }
            catch {
                throw;
            }
        }
    }
}