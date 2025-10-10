using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Facades.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.PaymentMethodRoot)]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly PaymentMethodFacade _paymentMethodFacade;

        public PaymentMethodController(PaymentMethodFacade paymentMethodFacade)
        {
            _paymentMethodFacade = paymentMethodFacade;
        }

        /// <summary>
        /// Lấy tất cả phương thức thanh toán
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<PaymentMethodDto>>>> GetAll()
        {
            try
            {
                var result = await _paymentMethodFacade.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<PaymentMethodDto>>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Lấy phương thức thanh toán theo ID
        /// </summary>
        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<PaymentMethodDto>>> GetById(int id)
        {
            try
            {
                var result = await _paymentMethodFacade.GetByIdAsync(id);
                return Ok(ResponseData<PaymentMethodDto>.Success(
                    StatusCodes.Status200OK,
                    result
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<PaymentMethodDto>.Error(
                    StatusCodes.Status404NotFound,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PaymentMethodDto>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Tạo mới phương thức thanh toán
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResponseData<PaymentMethodDto>>> Create([FromBody] PaymentMethodCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<PaymentMethodDto>.Error(
                    StatusCodes.Status400BadRequest,
                    "Dữ liệu không hợp lệ"
                ));

            try
            {
                var result = await _paymentMethodFacade.CreateAsync(request);
                return Ok(ResponseData<PaymentMethodDto>.Success(
                    StatusCodes.Status201Created,
                    result,
                    PaymentMethodMessages.SuccessfullyCreatedPaymentMethod
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<PaymentMethodDto>.Error(
                    StatusCodes.Status409Conflict,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PaymentMethodDto>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Cập nhật tên phương thức thanh toán
        /// </summary>
        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] PaymentMethodUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    "Dữ liệu không hợp lệ"
                ));

            try
            {
                await _paymentMethodFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    true,
                    PaymentMethodMessages.SuccessfullyUpdatedPaymentMethod
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(
                    StatusCodes.Status404NotFound,
                    ex.Message
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(
                    StatusCodes.Status409Conflict,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Xóa phương thức thanh toán
        /// </summary>
        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                await _paymentMethodFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    true,
                    PaymentMethodMessages.SuccessfullyDeletedPaymentMethod
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(
                    StatusCodes.Status404NotFound,
                    ex.Message
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(
                    StatusCodes.Status409Conflict,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }

        /// <summary>
        /// Cập nhật trạng thái phương thức thanh toán
        /// </summary>
        [HttpPatch("{id}/status/{newStatusId}")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int id, int newStatusId)
        {
            try
            {
                await _paymentMethodFacade.UpdateStatusAsync(id, newStatusId);
                return Ok(ResponseData<bool>.Success(
                    StatusCodes.Status200OK,
                    true,
                    PaymentMethodMessages.SuccessfullyUpdatedPaymentMethodStatus
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<bool>.Error(
                    StatusCodes.Status404NotFound,
                    ex.Message
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(
                    StatusCodes.Status409Conflict,
                    ex.Message
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(
                    StatusCodes.Status400BadRequest,
                    ex.Message
                ));
            }
        }
    }
}