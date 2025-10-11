using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Facades.purchaseorders;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly PurchaseOrderFacade _purchaseOrderFacade;

        public PurchaseOrderController(PurchaseOrderFacade purchaseOrderFacade)
        {
            _purchaseOrderFacade = purchaseOrderFacade;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _purchaseOrderFacade.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _purchaseOrderFacade.GetByIdAsync(id);
            if (result == null)
                return NotFound("Không tìm thấy đơn nhập hàng.");

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] PurchaseOrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _purchaseOrderFacade.CreateAsync(request);
            return result
                ? Ok("Tạo đơn nhập hàng thành công.")
                : BadRequest("Không thể tạo đơn nhập hàng.");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PurchaseOrderUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _purchaseOrderFacade.UpdateAsync(id, request);
                return result ? Ok("Cập nhật thành công.") : BadRequest("Không thể cập nhật đơn nhập hàng.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.DeleteAsync(id);
                return result ? Ok("Xóa đơn nhập hàng thành công.") : BadRequest("Không thể xóa đơn nhập hàng.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}/status/{statusId}")]
        public async Task<IActionResult> UpdateStatusAsync(int id, int statusId)
        {
            try
            {
                var result = await _purchaseOrderFacade.UpdateStatusAsync(id, statusId);
                return result ? Ok("Cập nhật trạng thái thành công.") : BadRequest("Không thể cập nhật trạng thái.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("{poId}/items")]
        public async Task<IActionResult> AddItemAsync(int poId, [FromBody] PurchaseOrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _purchaseOrderFacade.AddItemAsync(poId, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{poId}/items/{itemId}")]
        public async Task<IActionResult> UpdateItemAsync(int poId, int itemId, [FromBody] PurchaseOrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _purchaseOrderFacade.UpdateItemAsync(poId, itemId, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{poId}/items/{itemId}")]
        public async Task<IActionResult> DeleteItemAsync(int poId, int itemId)
        {
            try
            {
                var result = await _purchaseOrderFacade.DeleteItemAsync(poId, itemId);
                return result ? Ok("Xóa item thành công.") : BadRequest("Không thể xóa item.");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
