using AutoMapper;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.product_images;

namespace ec_project_api.Facades.products {
    public class ProductImageFacade {
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;

        public ProductImageFacade(IProductImageService productImageService, IMapper mapper) {
            _productImageService = productImageService;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductImageDetailDto>> GetAllByProductIdAsync(int productId) {
            var productImages = await _productImageService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductImageDetailDto>>(productImages);
        }

        public async Task<bool> UploadSingleProductImageAsync(int productId, ProductImageRequest request) {
            if (request.FileImage == null || request.FileImage.Length == 0) {
                return false;
            }

            var productImage = new ProductImage
            {
                ProductId = productId,
                AltText = request.AltText,
                IsPrimary = request.IsPrimary,
            };

            var result = await _productImageService.UploadSingleProductImageAsync(productImage, request.FileImage);

            return result;
        }
    }
}
