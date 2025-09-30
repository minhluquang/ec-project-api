using AutoMapper;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.product_images;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;

namespace ec_project_api.Facades.products {
    public class ProductFacade {
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductFacade(IProductService productService, IProductImageService productImageService, IStatusService statusService, IMaterialService materialService, ICategoryService categoryService, IMapper mapper) {
            _productService = productService;
            _statusService = statusService;
            _categoryService = categoryService;
            _materialService = materialService;
            _mapper = mapper;
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
            var existingProduct = await _productService.FirstOrDefaultAsync(p => (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId) || p.Slug == request.Slug);

            if (existingProduct != null) {
                if (existingProduct.Slug == existingProduct.Slug)
                    throw new InvalidOperationException(ProductMessages.ProductSlugAlreadyExists);
                else
                    throw new InvalidOperationException(ProductMessages.ProductAlreadyExistsWithNameCategoryMaterial);
            }

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Product && s.Name == StatusVariables.Inactive);
            if (inactiveStatus == null) throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var product = _mapper.Map<Product>(request);
            product.StatusId = inactiveStatus.StatusId;

            var productImage = new ProductImage
            {
                ProductId = product.ProductId,
                AltText = request.AltText,
                IsPrimary = true,
                DisplayOrder = 1
            };

            return await _productService.CreateAsync(product, productImage, request.FileImage);
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateRequest request) {
            var existingProduct = await _productService.FirstOrDefaultAsync(
                p => (p.ProductId != id) &&
                     (
                        (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId)
                        || (p.Slug == request.Slug)
                     )
            );

            if (existingProduct != null)
                if (existingProduct.Slug == request.Slug)
                    throw new InvalidOperationException(ProductMessages.ProductSlugAlreadyExists);
                else
                    throw new InvalidOperationException(ProductMessages.ProductAlreadyExists);

            var currentProduct = await _productService.GetByIdAsync(id);
            if (currentProduct == null)
                throw new KeyNotFoundException(ProductMessages.ProductAlreadyExists);

            var material = await _materialService.GetByIdAsync(request.MaterialId);
            if (material == null)
                throw new InvalidOperationException(MaterialMessages.MaterialNotFound);
            var category = await _categoryService.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Product)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);


            _mapper.Map(request, currentProduct);
            currentProduct.UpdatedAt = DateTime.UtcNow;

            return await _productService.UpdateAsync(currentProduct);
        }
    }
}
