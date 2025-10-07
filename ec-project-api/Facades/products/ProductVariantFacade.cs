using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using Microsoft.IdentityModel.Tokens;

namespace ec_project_api.Facades.products {
    public class ProductVariantFacade {
        private readonly IProductVariantService _productVariantService;
        private readonly IProductService _productService;
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ProductVariantFacade(IProductVariantService productVariantService, IProductService productService, IColorService colorService, ISizeService sizeService, IStatusService statusService,
            IMapper mapper) {
            _productVariantService = productVariantService;
            _productService = productService;
            _colorService = colorService;
            _sizeService = sizeService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetAllByProductIdAsync(int productId) {
            var productVariants = await _productVariantService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(productVariants);
        }

        public async Task<bool> CreateAsync(int productId, ProductVariantCreateRequest request) {
            var product = await _productService.GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            var size = await _sizeService.GetByIdAsync(request.SizeId);
            if (size == null)
                throw new KeyNotFoundException(SizeMessages.InvalidSizeData);

            var existingProductVariant = product.ProductVariants.Any(pv => (pv.SizeId == request.SizeId));

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.ProductVariant && s.Name == StatusVariables.Inactive);
            if (inactiveStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            if (existingProductVariant)
                throw new InvalidOperationException(ProductMessages.ProductVariantAlreadyExists);

            if (product.Category.Slug.IsNullOrEmpty())
                throw new ArgumentException(ProductMessages.ProductCategorySlugNotFound);

            string skuCategory = string.Concat(
                product.Category.Slug.Trim()
                    .Split("-", StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => char.ToUpper(part[0]))
            );

            string sku = $"YAM{productId:D3}-{skuCategory}-{size.Name}";

            var productVariant = _mapper.Map<ProductVariant>(request);
            productVariant.ProductId = productId;
            productVariant.Sku = sku;
            productVariant.StatusId = inactiveStatus.StatusId;

            return await _productVariantService.CreateAsync(productVariant);
        }

        public async Task<bool> UpdateAsync(int productId, int productVariantId, ProductVariantUpdateRequest request) {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null) {
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);
            }

            var productVariant = product.ProductVariants.FirstOrDefault(pv => pv.ProductVariantId == productVariantId);
            if (productVariant == null || productVariant.ProductId != productId)
                throw new KeyNotFoundException(ProductMessages.ProductVariantNotFound);
            else {
                if (productVariant.SizeId == request.SizeId && productVariant.StatusId == request.StatusId)
                    throw new ArgumentException(ProductMessages.NoChangeDataToUpdate);
            }

            var size = await _sizeService.GetByIdAsync(request.SizeId);
            if (size == null)
                throw new KeyNotFoundException(SizeMessages.InvalidSizeData);

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.ProductVariant)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            _mapper.Map(request, productVariant);
            productVariant.UpdatedAt = DateTime.UtcNow;
            return await _productVariantService.UpdateAsync(productVariant);
        }
    }
}