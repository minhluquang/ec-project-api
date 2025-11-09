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
        private readonly ISizeService _sizeService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ProductVariantFacade(IProductVariantService productVariantService, IProductService productService, IColorService colorService, ISizeService sizeService, IStatusService statusService,
            IMapper mapper) {
            _productVariantService = productVariantService;
            _productService = productService;
            _sizeService = sizeService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVariantDetailDto>> GetAllByProductIdAsync(int productId) {
            var productVariants = await _productVariantService.GetAllByProductIdAsync(productId);
            var mappedVariants = _mapper.Map<IEnumerable<ProductVariantDetailDto>>(productVariants);

            foreach (var variant in mappedVariants) {
                var originalVariant = productVariants.First(pv => pv.ProductVariantId == variant.ProductVariantId);
                variant.canDelete = !originalVariant.OrderItems.Any() && originalVariant.StockQuantity == 0 && originalVariant.Status.Name == StatusVariables.Inactive;
            }

            return mappedVariants;
        }

        public async Task<bool> CreateAsync(int productId, ProductVariantCreateRequest request) {
            var product = await _productService.GetByIdAsync(productId) ??
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            var size = await _sizeService.GetByIdAsync(request.SizeId) ??
                throw new KeyNotFoundException(SizeMessages.InvalidSizeData);

            var existingProductVariant = product.ProductVariants.Any(pv => (pv.SizeId == request.SizeId));

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.ProductVariant && s.Name == StatusVariables.Inactive) ??
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

            string sku = $"YAM{productId:D3}-{skuCategory}-{size.Name}-{product.ColorId}";

            var productVariant = _mapper.Map<ProductVariant>(request);
            productVariant.ProductId = productId;
            productVariant.Sku = sku;
            productVariant.StatusId = inactiveStatus.StatusId;

            return await _productVariantService.CreateAsync(productVariant);
        }

        public async Task<bool> UpdateAsync(int productId, int productVariantId, ProductVariantUpdateRequest request) {
            var product = await _productService.GetByIdAsync(productId) ??
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            var productVariant = product.ProductVariants.FirstOrDefault(pv => pv.ProductVariantId == productVariantId);
            if (productVariant == null || productVariant.ProductId != productId)
                throw new KeyNotFoundException(ProductMessages.ProductVariantNotFound);
            else {
                if (productVariant.SizeId == request.SizeId && productVariant.StatusId == request.StatusId)
                    throw new ArgumentException(ProductMessages.NoChangeDataToUpdate);
            }

            var size = await _sizeService.GetByIdAsync(request.SizeId) ??
                throw new KeyNotFoundException(SizeMessages.InvalidSizeData);
            var duplicateSizeExists = product.ProductVariants.Any(pv => pv.SizeId == request.SizeId && pv.ProductVariantId != productVariantId);
            if (duplicateSizeExists)
                throw new InvalidOperationException(ProductMessages.ProductVariantAlreadyExists);
            
            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.ProductVariant)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            
            var transitioningToActive = status.Name == StatusVariables.Active &&
                                            (productVariant.Status == null || productVariant.Status.Name == StatusVariables.Inactive);
            if (transitioningToActive) {
                var sizeActiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Size && s.Name == StatusVariables.Active)
                                       ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

                if (size.Status == null || size.Status.StatusId != sizeActiveStatus.StatusId) {
                    size.StatusId = sizeActiveStatus.StatusId;
                    size.UpdatedAt = DateTime.UtcNow;
                    await _sizeService.UpdateAsync(size);
                }
            }

            _mapper.Map(request, productVariant);
            productVariant.UpdatedAt = DateTime.UtcNow;
            return await _productVariantService.UpdateAsync(productVariant);
        }

        public async Task<bool> DeleteAsync(int productId, int productVariantId) {
            var productVariant = await _productVariantService.GetByIdAsync(productVariantId) ??
                throw new KeyNotFoundException(ProductMessages.ProductVariantNotFound);

            if (productVariant.ProductId != productId)
                throw new ArgumentException(ProductMessages.ProductVariantNotBelongToProduct);

            if (productVariant.Status?.Name != StatusVariables.Inactive || productVariant.StockQuantity != 0)
                throw new InvalidOperationException(ProductMessages.ProductVariantDeleteFailed);

            return await _productVariantService.DeleteAsync(productVariant);
        }
    }
}