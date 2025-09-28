using AutoMapper;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.product_images;

namespace ec_project_api.Facades.products {
    public class ProductFacade {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;
        private readonly DataContext _dbContext;

        public ProductFacade(IProductService productService, IProductImageService productImageService, IStatusService statusService, IMapper mapper, DataContext dbContext) {
            _productService = productService;
            _productImageService = productImageService;
            _statusService = statusService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync() {
            var products = await _productService.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDetailDto> GetByIdAsync(int id) {
            var product = await _productService.GetByIdAsync(id);
            return _mapper.Map<ProductDetailDto>(product);
        }

        public async Task<bool> CreateAsync(ProductCreateRequest request) {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try {
                var existingProduct = await _productService.FirstOrDefaultAsync(p => (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId) || p.Slug == request.Slug);

                if (existingProduct != null) {
                    if (existingProduct.Slug == existingProduct.Slug) {
                        throw new InvalidOperationException("Slug sản phẩm đã tồn tại");
                    }
                    else {
                        throw new InvalidOperationException("Sản phẩm đã tồn tại với tên, thể loại và chất liệu này");
                    }
                }

                var inactiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == "Product" && s.Name == "Inactive");
                if (inactiveStatus == null) throw new InvalidOperationException("Tạo sản phẩm thất bại do không tìm thấy trạng thái sản phẩm");

                var product = _mapper.Map<Product>(request);
                product.StatusId = inactiveStatus.StatusId;

                var result = await _productService.CreateAsync(product);
                if (!result) throw new Exception("Tạo sản phẩm thất bại");

                var productImage = new ProductImage
                {
                    ProductId = product.ProductId,
                    AltText = request.AltText,
                    IsPrimary = true,
                    DisplayOrder = 1
                };
                var uploadResult = await _productImageService.UploadSingleProductImageAsync(productImage, request.FileImage);
                if (!uploadResult) throw new Exception("Upload ảnh thất bại");

                await transaction.CommitAsync();
                return true;
            }
            catch {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
