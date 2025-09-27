using AutoMapper;
using ec_project_api.Dtos.response.products;
using ec_project_api.Services;

namespace ec_project_api.Facades.products
{
    public class ProductFacade
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductFacade(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDetailDto> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            return _mapper.Map<ProductDetailDto>(product);
        }
    }

}
