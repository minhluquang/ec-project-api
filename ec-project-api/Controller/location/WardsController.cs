using ec_project_api.Interfaces.location;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.location
{
    [ApiController]
    [Route("api/[controller]")]
    public class WardsController : ControllerBase
    {
        private readonly IWardService _wardService;

        public WardsController(IWardService wardService)
        {
            _wardService = wardService;
        }


        [HttpGet("by-province/{provinceId}")]
        public async Task<IActionResult> GetWardsByProvinceId(int provinceId)
        {
            try
            {
                var wards = await _wardService.GetWardsByProvinceIdAsync(provinceId);
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách phường/xã thành công",
                    data = wards
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi lấy danh sách phường/xã",
                    error = ex.Message
                });
            }
        }
    }
}
