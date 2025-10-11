using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.response;
using ec_project_api.Facades.system;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.system
{
    [Route(PathVariables.DashboardRoot)]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardFacade _dashboardFacade;

        public DashboardController(DashboardFacade dashboardFacade)
        {
            _dashboardFacade = dashboardFacade;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<ResponseData<DashboardOverviewDto>>> GetOverview()
        {
            return await HandleRequestAsync(async () =>
            {
                var result = await _dashboardFacade.GetOverviewAsync();
                return ResponseData<DashboardOverviewDto>.Success(StatusCodes.Status200OK, result, "Dashboard overview retrieved successfully");
            });
        }

        private async Task<ActionResult<ResponseData<T>>> HandleRequestAsync<T>(Func<Task<ResponseData<T>>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<T>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseData<T>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseData<T>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
namespace ec_project_api.Dtos.response
{
    public class DashboardOverviewDto
    {
        public decimal MonthlyRevenue { get; set; }
        public decimal RevenueChangePercent { get; set; }
        public int TotalOrders { get; set; }
        public decimal OrderChangePercent { get; set; }
        public int NewCustomers { get; set; }
        public decimal CustomerChangePercent { get; set; }
        public int ProductsSold { get; set; }
        public decimal ProductChangePercent { get; set; }
    }
}

