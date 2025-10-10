using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Facades.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierFacade _supplierFacade;

        public SupplierController(SupplierFacade supplierFacade)
        {
            _supplierFacade =  supplierFacade;    
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var result = await _supplierFacade.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _supplierFacade.GetByIdAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] SupplierCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _supplierFacade.CreateAsync(request);
                return StatusCode(StatusCodes.Status201Created, new { message = "Tạo nhà cung cấp thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] SupplierUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _supplierFacade.UpdateAsync(id, request);
                return Ok(new { message = "Cập nhật nhà cung cấp thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _supplierFacade.DeleteAsync(id);
                return Ok(new { message = "Xóa nhà cung cấp thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }
    }
}
