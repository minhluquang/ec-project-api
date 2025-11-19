using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.dashboard;
using ec_project_api.Facades.system;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.dashboard
{
    [Route(PathVariables.DashboardRoot)]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly DashboardFacade _dashboardFacade;

        public DashboardController(DashboardFacade dashboardFacade)
        {
            _dashboardFacade = dashboardFacade;
        }

        [HttpGet("overview")]
        [Authorize(Policy = "Dashboard.GetOverview")]
        public async Task<ActionResult<ResponseData<DashboardOverviewDto>>> GetOverview()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _dashboardFacade.GetOverviewAsync();
                return ResponseData<DashboardOverviewDto>.Success(StatusCodes.Status200OK, result,
                    "Dashboard overview retrieved successfully");
            });
        }

        [HttpGet("monthly-revenue")]
        [Authorize(Policy = "Dashboard.GetMonthlyRevenue")]
        public async Task<ActionResult<ResponseData<MonthlyRevenueResponse>>> GetMonthlyRevenue([FromQuery] string timeRange = "current_month")
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _dashboardFacade.GetMonthlyRevenueAsync(timeRange);
                return ResponseData<MonthlyRevenueResponse>.Success(StatusCodes.Status200OK, result,
                    "Monthly revenue retrieved successfully");
            });
        }

        [HttpGet("category-sales-percentage")]
        [Authorize(Policy = "Dashboard.GetCategorySalesPercentage")]
        public async Task<ActionResult<ResponseData<List<CategorySalesPercentageDto>>>> GetCategorySalesPercentage(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? preset = null)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _dashboardFacade.GetCategorySalesPercentageAsync(startDate, endDate, preset);
                return ResponseData<List<CategorySalesPercentageDto>>.Success(
                    StatusCodes.Status200OK, 
                    result,
                    "Category sales percentage retrieved successfully");
            });
        }

        [HttpGet("monthly-revenue-stats")]
        [Authorize(Policy = "Dashboard.GetMonthlyRevenueStats")]
        public async Task<ActionResult<ResponseData<List<MonthlyRevenueStatsDto>>>> GetMonthlyRevenueStats([FromQuery] int year = 0)
        {
            return await ExecuteAsync(async () =>
            {
                var selectedYear = year == 0 ? DateTime.UtcNow.Year : year;
                var result = await _dashboardFacade.GetMonthlyRevenueStatsAsync(selectedYear);
                return ResponseData<List<MonthlyRevenueStatsDto>>.Success(
                    StatusCodes.Status200OK,
                    result,
                    "Monthly revenue stats retrieved successfully");
            });
        }

        [HttpGet("top-selling-products")]
        [Authorize(Policy = "Dashboard.GetTopSellingProducts")]
        public async Task<ActionResult<ResponseData<List<TopSellingProductDto>>>> GetTopSellingProducts(
            [FromQuery] int top = 10,
            [FromQuery] int year = 0)
        {
            return await ExecuteAsync(async () =>
            {
                var selectedYear = year == 0 ? DateTime.UtcNow.Year : year;
                var result = await _dashboardFacade.GetTopSellingProductsAsync(top, selectedYear);
                return ResponseData<List<TopSellingProductDto>>.Success(
                    StatusCodes.Status200OK,
                    result,
                    "Top selling products retrieved successfully");
            });
        }

        [HttpGet("weekly-sales")]
        [Authorize(Policy = "Dashboard.GetWeeklySales")]
        public async Task<ActionResult<ResponseData<List<DailySalesDto>>>> GetWeeklySales()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _dashboardFacade.GetWeeklySalesAsync();
                return ResponseData<List<DailySalesDto>>.Success(
                    StatusCodes.Status200OK,
                    result,
                    "Weekly sales retrieved successfully");
            });
        }

        [HttpGet("monthly-profit")]
        [Authorize(Policy = "Dashboard.GetMonthlyProfit")]
        public async Task<ActionResult<ResponseData<List<MonthlyProfitDto>>>> GetMonthlyProfit(
            [FromQuery] int year = 0)
        {
            return await ExecuteAsync(async () =>
            {
                var selectedYear = year == 0 ? DateTime.UtcNow.Year : year;
                var result = await _dashboardFacade.GetMonthlyProfitAsync(selectedYear);
                return ResponseData<List<MonthlyProfitDto>>.Success(
                    StatusCodes.Status200OK,
                    result,
                    "Monthly profit retrieved successfully");
            });
        }
    }
}
