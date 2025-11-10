using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.purchaseorders;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.purchaseorders;
using ec_project_api.Facades.purchaseorders;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "PurchaseOrder.GetAll")]
        public async Task<ActionResult<ResponseData<PagedResult<PurchaseOrderResponse>>>> GetAllAsync([FromQuery] PurchaseOrderFilter filter)
        {
            try
            {
                var result = await _purchaseOrderFacade.GetAllPagedAsync(filter?? new PurchaseOrderFilter());
                return Ok(ResponseData<PagedResult<PurchaseOrderResponse>>.Success(
                    StatusCodes.Status200OK, result, PurchaseOrderMessages.PurchaseOrderRetrievedSuccessfully));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PagedResult<PurchaseOrderResponse>>.Error(
                    StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ResponseData<PurchaseOrderStatisticsResponse>>> GetStatisticsAsync(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? supplierId = null)
        {
            try
            {
                var result = await _purchaseOrderFacade.GetStatisticsAsync(startDate, endDate, supplierId);
                return Ok(ResponseData<PurchaseOrderStatisticsResponse>.Success(
                    StatusCodes.Status200OK, result, "Lấy thống kê đơn nhập hàng thành công."));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PurchaseOrderStatisticsResponse>.Error(
                    StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        [Authorize(Policy = "PurchaseOrder.GetById")]
        public async Task<ActionResult<ResponseData<PurchaseOrderResponse>>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.GetByIdAsync(id);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderResponse>.Error(
                        StatusCodes.Status404NotFound, PurchaseOrderMessages.PurchaseOrderNotFound));

                return Ok(ResponseData<PurchaseOrderResponse>.Success(
                    StatusCodes.Status200OK, result, PurchaseOrderMessages.PurchaseOrderRetrievedSuccessfully));
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
        [Authorize(Policy = "PurchaseOrder.Create")]
        public async Task<ActionResult<ResponseData<bool>>> CreateAsync([FromBody] PurchaseOrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.CreateAsync(request);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, PurchaseOrderMessages.PurchaseOrderCreatedSuccessfully))
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
        [Authorize(Policy = "PurchaseOrder.Update")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateAsync(int id, [FromBody] PurchaseOrderUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.UpdateAsync(id, request);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.PurchaseOrderUpdatedSuccessfully))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderUpdateFailed));
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
        [Authorize(Policy = "PurchaseOrder.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> DeleteAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.DeleteAsync(id);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.PurchaseOrderDeletedSuccessfully))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderDeleteFailed));
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
        [Authorize(Policy = "PurchaseOrder.SetStatus")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatusAsync(int id, short statusId)
        {
            try
            {
                var result = await _purchaseOrderFacade.UpdateStatusAsync(id, statusId);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.PurchaseOrderStatusUpdatedSuccessfully))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.PurchaseOrderStatusUpdateFailed));
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

        [HttpPut(PathVariables.GetById + "/cancel")]
        [Authorize(Policy = "PurchaseOrder.SetStatus")]
        public async Task<ActionResult<ResponseData<bool>>> CancelAsync(int id)
        {
            try
            {
                var result = await _purchaseOrderFacade.CancelAsync(id);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Hủy đơn nhập hàng thành công."))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể hủy đơn nhập hàng."));
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

        [HttpPost("{poId}/items")]
        public async Task<ActionResult<ResponseData<PurchaseOrderItemResponse>>> AddItemAsync(int poId, [FromBody] PurchaseOrderItemCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.AddItemAsync(poId, request);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, PurchaseOrderMessages.ProductVariantNotFound));

                return Ok(ResponseData<PurchaseOrderItemResponse>.Success(StatusCodes.Status200OK, result, PurchaseOrderMessages.ItemAddedSuccessfully));
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
                return BadRequest(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status400BadRequest,
                    string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _purchaseOrderFacade.UpdateItemAsync(poId, itemId, request);
                if (result == null)
                    return NotFound(ResponseData<PurchaseOrderItemResponse>.Error(StatusCodes.Status404NotFound, PurchaseOrderMessages.ProductVariantNotFound));

                return Ok(ResponseData<PurchaseOrderItemResponse>.Success(StatusCodes.Status200OK, result, PurchaseOrderMessages.ItemUpdatedSuccessfully));
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
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, PurchaseOrderMessages.ItemDeletedSuccessfully))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PurchaseOrderMessages.ItemDeleteFailed));
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
