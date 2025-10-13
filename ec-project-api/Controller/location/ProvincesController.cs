using ec_project_api.Interfaces.location;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.location
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvincesController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllProvinces()
        {
            try
            {
                var provinces = await _provinceService.GetAllProvincesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách tỉnh/thành phố thành công",
                    data = provinces
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi lấy danh sách tỉnh/thành phố",
                    error = ex.Message
                });
            }
        }
    }
}

