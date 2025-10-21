using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Facades.payments;
using ec_project_api.Facades.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ec_project_api.Controllers.Payments
{
    [Route(PathVariables.PaymentDestinationRoot)]
    [ApiController]
    public class PaymentDestinationController : BaseController
    {
        private readonly PaymentDestinationFacade _facade;

        public PaymentDestinationController(PaymentDestinationFacade facade)
        {
            _facade = facade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<PaymentDestinationDto>>>> GetAll([FromQuery] PaymentDestinationFilter filter)
        {
            try
            {
                var result = await _facade.GetAllPagedAsync(filter);
                return Ok(ResponseData<PagedResult<PaymentDestinationDto>>.Success(StatusCodes.Status200OK,result,PaymentMessages.PaymentDestinationRetrievedSuccessfully
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PagedResult<PaymentDestinationDto>>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentProcessingError} {ex.Message}"
                ));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<PaymentDestinationDto>>> GetById(int id)
        {
            try
            {
                var dto = await _facade.GetByIdAsync(id);
                if (dto == null)
                {
                    return NotFound(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status404NotFound,PaymentMessages.NotFound
                    ));
                }

                return Ok(ResponseData<PaymentDestinationDto>.Success(StatusCodes.Status200OK, dto, PaymentMessages.PaymentDestinationRetrievedSuccessfully
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentProcessingError} {ex.Message}"
                ));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] PaymentDestinationCreateRequest request)
        {
            try
            {
                var result = await _facade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created,true,PaymentMessages.PaymentDestinationCreatedSuccessfully
                    ));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,PaymentMessages.PaymentDestinationCreatedFailed
                    ));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentMessages.DuplicatePaymentDestination} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentProcessingError} {ex.Message}"
                ));
            }
        }

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] PaymentDestinationUpdateRequest request)
        {
            try
            {
                var result = await _facade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result, PaymentMessages.PaymentDestinationUpdatedSuccessfully
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentMessages.DuplicatePaymentDestination} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentDestinationUpdatedFailed} {ex.Message}"
                ));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                var result = await _facade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result,PaymentMessages.PaymentDestinationDeletedSuccessfully
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentMessages.PaymentDestinationDeletedFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentProcessingError} {ex.Message}"
                ));
            }
        }

        [HttpPatch(PathVariables.GetById + "/status/{statusId}")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int id, short statusId)
        {
            try
            {
                var result = await _facade.UpdateStatusAsync(id, statusId);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result,PaymentMessages.PaymentDestinationStatusUpdatedSuccessfully
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, $"{PaymentMessages.PaymentDestinationStatusUpdatedFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentMessages.PaymentProcessingError} {ex.Message}"
                ));
            }
        }
    }
}
