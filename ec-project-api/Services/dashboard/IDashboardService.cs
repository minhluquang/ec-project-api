
using ec_project_api.Dtos.response.dashboard;

namespace ec_project_api.Services.custom
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(string timeRange);
        Task<List<CategorySalesPercentageDto>> GetCategorySalesPercentageAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? preset);
        Task<List<MonthlyRevenueStatsDto>> GetMonthlyRevenueStatsAsync(int year);
        Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top, int year);
        Task<List<DailySalesDto>> GetWeeklySalesAsync();
        Task<List<MonthlyProfitDto>> GetMonthlyProfitAsync(int year);
    }
}

