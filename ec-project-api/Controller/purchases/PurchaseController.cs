using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.purchaseorders;
using ec_project_api.Facades.purchaseorders;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.PurchaseOrder)]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly PurchaseOrderFacade _purchaseOrderFacade;

        public PurchaseOrderController(PurchaseOrderFacade purchaseOrderFacade)
        {
            _purchaseOrderFacade = purchaseOrderFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<PurchaseOrderResponse>>>> GetAllAsync([FromQuery] Dtos.request.purchaseorders.PurchaseOrderFilter? filter = null)
        {
            try
            {
                var result = await _purchaseOrderFacade.GetAllAsync(filter);
                return Ok(ResponseData<IEnumerable<PurchaseOrderResponse>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<PurchaseOrderResponse>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<PurchaseOrderResponse>>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.GetByIdAsync(id);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderResponse>.Error(StatusCodes.Status404NotFound, PurchaseOrderMessages.PurchaseOrderNotFound));

                return Ok(ResponseData<PurchaseOrderResponse>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<PurchaseOrderResponse>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PurchaseOrderResponse>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> CreateAsync([FromBody] PurchaseOrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.CreateAsync(request);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, PurchaseOrderMessages.PurchaseOrderCreateFailed.Replace("Không thể", "Tạo")))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderCreateFailed));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> UpdateAsync(int id, [FromBody] PurchaseOrderUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.UpdateAsync(id, request);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.PurchaseOrderUpdateFailed.Replace("Không thể", "Cập nhật"))) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderUpdateFailed));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> DeleteAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.DeleteAsync(id);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.PurchaseOrderDeleteFailed.Replace("Không thể", "Xóa"))) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderDeleteFailed));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPut(PathVariables.GetById + "/status/{statusId}")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatusAsync(int id, short statusId)
        {
            try
            {
                var result = await _purchaseOrderFacade.UpdateStatusAsync(id, statusId);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Cập nhật trạng thái thành công.")) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể cập nhật trạng thái."));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost("{poId}/items")]
        public async Task<ActionResult<ResponseData<PurchaseOrderItemResponse>>> AddItemAsync(int poId, [FromBody] PurchaseOrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.AddItemAsync(poId, request);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, PurchaseOrderMessages.ProductVariantNotFound));

                return Ok(ResponseData<PurchaseOrderItemResponse>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPut("{poId}/items/{itemId}")]
        public async Task<ActionResult<ResponseData<PurchaseOrderItemResponse>>> UpdateItemAsync(int poId, int itemId, [FromBody] PurchaseOrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.UpdateItemAsync(poId, itemId, request);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, PurchaseOrderMessages.ProductVariantNotFound));

                return Ok(ResponseData<PurchaseOrderItemResponse>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete("{poId}/items/{itemId}")]
        public async Task<ActionResult<ResponseData<bool>>> DeleteItemAsync(int poId, int itemId)
        {
            try
            {
                var result = await _purchaseOrderFacade.DeleteItemAsync(poId, itemId);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Xóa item thành công.")) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể xóa item."));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
