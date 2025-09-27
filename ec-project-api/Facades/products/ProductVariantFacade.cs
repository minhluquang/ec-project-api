using AutoMapper;
using ec_project_api.Dtos.response.products;
using ec_project_api.Services;

namespace ec_project_api.Facades.products
{
    public class ProductVariantFacade
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IMapper _mapper;

        public ProductVariantFacade(IProductVariantService productVariantService, IMapper mapper)
        {
            _productVariantService = productVariantService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetAllByProductIdAsync(int productId)
        {
            var productVariants = await _productVariantService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductVariantDto>>(productVariants);
        }
    }
}
