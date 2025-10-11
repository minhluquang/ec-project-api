using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.dashboard;

namespace ec_project_api.Services.custom
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(string timeRange);
    }
}

