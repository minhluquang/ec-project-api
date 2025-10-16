using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.response.homepage;
using ec_project_api.Interfaces;

namespace ec_project_api.Facades.Homepage
{
    public class HomepageFacade
    {
        private readonly IHomepageService _homepageService;
        private readonly IMapper _mapper;

        public HomepageFacade(IHomepageService homepageService, IMapper mapper)
        {
            _homepageService = homepageService;
            _mapper = mapper;
        }
        public async Task<HomepageDto> GetHomepageDataAsync()
        {
            var result = await _homepageService.GetHomepageDataAsync();

            if (result == null)
                throw new InvalidOperationException(HomepageMessages.DataNotFound);

            return result;
        }
        public async Task<List<CategoryHomePageDto>> GetCategoriesAsync()
        {
            var categories = await _homepageService.GetCategoriesAsync();

            if (categories == null || categories.Count == 0)
                throw new InvalidOperationException(HomepageMessages.NoCategoriesFound);

            return categories;
        }
        public async Task<List<ProductSummaryDto>> GetBestSellingProductsAsync()
        {
            var products = await _homepageService.GetBestSellingProductsAsync();

            if (products == null || products.Count == 0)
                throw new InvalidOperationException(HomepageMessages.NoBestSellingFound);

            return products;
        }
        public async Task<List<ProductSummaryDto>> GetOnSaleProductsAsync()
        {
            var products = await _homepageService.GetOnSaleProductsAsync();

            if (products == null || products.Count == 0)
                throw new InvalidOperationException(HomepageMessages.NoOnSaleFound);

            return products;
        }
    }
}
