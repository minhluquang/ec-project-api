using ec_project_api.Dtos.response.homepage;

namespace ec_project_api.Interfaces
{
    public interface IHomepageService
    {
        Task<HomepageDto> GetHomepageDataAsync();
        Task<List<CategoryHomePageDto>> GetCategoriesAsync();
        Task<List<ProductSummaryDto>> GetBestSellingProductsAsync();
        Task<List<ProductSummaryDto>> GetOnSaleProductsAsync();
        Task<List<CategorySalesDto>> GetBestSellingCategoriesAsync();
    }
}
