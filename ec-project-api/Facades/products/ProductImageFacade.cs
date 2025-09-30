using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services.product_images;
using System.Linq;

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

        public async Task<bool> UpdateImageDisplayOrderAsync(int productId, List<ProductUpdateImageDisplayOrderRequest> request) {
            if (request.GroupBy(r => r.DisplayOrder).Any(g => g.Count() > 1))
                throw new InvalidOperationException(ProductMessages.ProductImageDisplayOrderConflict);

            var dbProductImages = (await _productImageService.GetAllByProductIdAsync(productId))
                .OrderBy(pi => pi.ProductImageId)
                .ToList();

            var reqImages = request
                .OrderBy(r => r.ProductImageId)
                .ToList();

            // Check enough quantity
            if (dbProductImages.Count != reqImages.Count)
                throw new InvalidOperationException(ProductMessages.ProductImageDisplayOrderNotEqual);

            // Check enough ids
            if (!dbProductImages.Select(x => x.ProductImageId).OrderBy(id => id).SequenceEqual(reqImages.Select(x => x.ProductImageId).OrderBy(id => id)))
                throw new InvalidOperationException(ProductMessages.ProductImageDisplayOrderNotEqual);

            return await _productImageService.UpdateImageDisplayOrderAsync(productId, request);
        }
    }
}
