using AutoMapper;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.product_images;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Services.product_groups;

namespace ec_project_api.Facades.products {
    public class ProductFacade {
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly IProductGroupService _productGroupService;
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;

        public ProductFacade(IProductService productService, IProductImageService productImageService, IStatusService statusService, IMaterialService materialService, ICategoryService categoryService, IProductGroupService productGroupService, IColorService colorService, IMapper mapper) {
            _productService = productService;
            _statusService = statusService;
            _categoryService = categoryService;
            _materialService = materialService;
            _productGroupService = productGroupService;
            _colorService = colorService;
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
            var existingProduct = await _productService.FirstOrDefaultAsync(p => (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId && p.ColorId == request.ColorId) || p.Slug == request.Slug);

            if (existingProduct != null) {
                if (existingProduct.Slug == request.Slug)
                    throw new InvalidOperationException(ProductMessages.ProductSlugAlreadyExists);
                else
                    throw new InvalidOperationException(ProductMessages.ProductAlreadyExistsWithNameCategoryMaterial);
            }

            var productGroup = await _productGroupService.GetByIdAsync(request.ProductGroupId) ??
                throw new InvalidOperationException(ProductMessages.ProductGroupNotFound);

            var material = await _materialService.GetByIdAsync(request.MaterialId) ??
                throw new InvalidOperationException(MaterialMessages.MaterialNotFound);

            var category = await _categoryService.GetByIdAsync(request.CategoryId) ??
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var color = await _colorService.GetByIdAsync(request.ColorId) ??
                throw new InvalidOperationException(ColorMessages.ColorNotFound);

            var draftStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Product && s.Name == StatusVariables.Draft) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var product = _mapper.Map<Product>(request);
            product.StatusId = draftStatus.StatusId;
            product.DiscountPercentage = 0;
            product.BasePrice = 0;

            var productImage = new ProductImage
            {
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

            var currentProduct = await _productService.GetByIdAsync(id) ??
                throw new KeyNotFoundException(ProductMessages.ProductAlreadyExists);

            var material = await _materialService.GetByIdAsync(request.MaterialId) ??
                throw new InvalidOperationException(MaterialMessages.MaterialNotFound);

            var category = await _categoryService.GetByIdAsync(request.CategoryId) ??
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Product)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);


            _mapper.Map(request, currentProduct);
            currentProduct.UpdatedAt = DateTime.UtcNow;

            return await _productService.UpdateAsync(currentProduct);
        }

        public async Task<IEnumerable<ProductDto>> GetAllByCategoryidAsync(short categoryId, int? pageNumber, int? pageSize, decimal? minPrice, decimal? maxPrice, short? colorId, string? orderBy) {
            if (pageNumber <= 0 && pageNumber.HasValue)
                throw new ArgumentException("Số trang phải lớn hơn 0", nameof(pageNumber));

            if ((pageSize <= 0 || pageSize > 100) && pageSize.HasValue)
                throw new ArgumentException("Kích thước trang phải từ 1 đến 100", nameof(pageSize));

            if (minPrice.HasValue && minPrice.Value < 0)
                throw new ArgumentException("Giá tối thiểu phải lớn hơn hoặc bằng 0", nameof(minPrice));

            if (maxPrice.HasValue && maxPrice.Value < 0)
                throw new ArgumentException("Giá tối đa phải lớn hơn hoặc bằng 0", nameof(maxPrice));

            if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
                throw new ArgumentException("Giá tối thiểu không thể lớn hơn giá tối đa");

            var category = await _categoryService.GetByIdAsync(categoryId);
            if (category == null) throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var products = await _productService.GetAllByCategoryidAsync(categoryId, pageNumber, pageSize, minPrice, maxPrice, colorId, orderBy);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<bool> DeleteAsync(int productId) {
            var product = await _productService.GetByIdAsync(productId) ??
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            if (product.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(ProductMessages.ProductDeleteFailedNotDraft);

            return await _productService.DeleteAsync(product);
        }

        public async Task<ProductFormMetaDto> GetProductFormMetaAsync() {
            return await _productService.GetProductFormMetaAsync();
        }
    }
}
