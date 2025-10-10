using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response;
using ec_project_api.DTOs.Payments;
using ec_project_api.Facades.payments;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.payments
{
    [Route(PathVariables.PaymentDestinationRoot)]
    [ApiController]
    public class PaymentDestinationController : ControllerBase
    {
        private readonly PaymentDestinationFacade _paymentDestinationFacade;

        public PaymentDestinationController(PaymentDestinationFacade paymentDestinationFacade)
        {
            _paymentDestinationFacade = paymentDestinationFacade;
        }

        // ✅ Lấy toàn bộ điểm đến thanh toán
        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<PaymentDestinationDto>>>> GetAll()
        {
            try
            {
                var result = await _paymentDestinationFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<PaymentDestinationDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<PaymentDestinationDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        // ✅ Lấy chi tiết 1 điểm đến thanh toán
        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<PaymentDestinationDto>>> GetById(int id)
        {
            try
            {
                var result = await _paymentDestinationFacade.GetByIdAsync(id);
                return Ok(ResponseData<PaymentDestinationDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        // ✅ Tạo mới điểm đến thanh toán
        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] PaymentDestinationCreateRequest request)
        {
            try
            {
                var result = await _paymentDestinationFacade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, PaymentDestinationMessages.SuccessfullyCreatedPaymentDestination));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PaymentDestinationMessages.PaymentDestinationCreateFailed));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        // ✅ Cập nhật thông tin ngân hàng (bank info)
        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> UpdateBankInfo(int id, [FromBody] PaymentDestinationUpdateRequest request)
        {
            try
            {
                var result = await _paymentDestinationFacade.UpdateBankInfoAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, PaymentDestinationMessages.SuccessfullyUpdatedPaymentDestination));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        // ✅ Cập nhật trạng thái (status)
        [HttpPatch("{id}/status/{newStatusId}")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int id, int newStatusId)
        {
            try
            {
                var result = await _paymentDestinationFacade.UpdateStatusAsync(id, newStatusId);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, PaymentDestinationMessages.SuccessfullyUpdatedPaymentDestinationStatus));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        // ✅ Xoá điểm đến thanh toán
        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                var result = await _paymentDestinationFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, PaymentDestinationMessages.SuccessfullyDeletedPaymentDestination));
            }
            catch (KeyNotFoundException ex)
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
