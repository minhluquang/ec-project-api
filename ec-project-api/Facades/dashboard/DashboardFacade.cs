using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.dashboard;
using ec_project_api.Services.custom;

namespace ec_project_api.Facades.system
{
    public class DashboardFacade
    {
        private readonly IDashboardService _dashboardService;

        public DashboardFacade(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            return await _dashboardService.GetOverviewAsync();
        }

        public async Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(string timeRange)
        {
            return await _dashboardService.GetMonthlyRevenueAsync(timeRange);
        }
        
        public async Task<List<CategorySalesPercentageDto>> GetCategorySalesPercentageAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? preset)
        {
            return await _dashboardService.GetCategorySalesPercentageAsync(startDate, endDate, preset);
        }
        
        public async Task<List<MonthlyRevenueStatsDto>> GetMonthlyRevenueStatsAsync(int year)
        {
            return await _dashboardService.GetMonthlyRevenueStatsAsync(year);
        }
        
        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top, int year)
        {
            return await _dashboardService.GetTopSellingProductsAsync(top, year);
        }
        
        public async Task<List<DailySalesDto>> GetWeeklySalesAsync()
        {
            return await _dashboardService.GetWeeklySalesAsync();
        }
        
        public async Task<List<MonthlyProfitDto>> GetMonthlyProfitAsync(int year)
        {
            return await _dashboardService.GetMonthlyProfitAsync(year);
        }
    }
}
