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
    }
}
