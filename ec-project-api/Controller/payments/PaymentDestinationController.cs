using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.payments;
using ec_project_api.DTOs.Payments;
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
        public async Task<ActionResult<ResponseData<IEnumerable<PaymentDestinationDto>>>> GetAll([FromQuery] PaymentDestinationFilter filter)
        {
            try
            {
                var result = await _facade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<PaymentDestinationDto>>.Success(StatusCodes.Status200OK,result, PaymentDestinationMessages.PaymentDestinationRetrievedSuccessfully
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<PaymentDestinationDto>>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationGetAllFailed} {ex.Message}"
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
                    return NotFound(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status404NotFound, PaymentDestinationMessages.PaymentDestinationNotFound
                    ));
                }

                return Ok(ResponseData<PaymentDestinationDto>.Success(StatusCodes.Status200OK, dto, PaymentDestinationMessages.PaymentDestinationRetrievedSuccessfully
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<PaymentDestinationDto>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationNotFound} {ex.Message}"
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
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created,true, PaymentDestinationMessages.SuccessfullyCreatedPaymentDestination
                    ));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, PaymentDestinationMessages.PaymentDestinationCreateFailed
                    ));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentDestinationMessages.PaymentDestinationCreateFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationCreateFailed} {ex.Message}"
                ));
            }
        }

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] PaymentDestinationUpdateRequest request)
        {
            try
            {
                var result = await _facade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result, PaymentDestinationMessages.SuccessfullyUpdatedPaymentDestinationStatus
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentDestinationMessages.PaymentDestinationUpdateFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationUpdateFailed} {ex.Message}"
                ));
            }
        }

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
        {
            try
            {
                var result = await _facade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result, PaymentDestinationMessages.SuccessfullyDeletedPaymentDestination
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict,$"{PaymentDestinationMessages.PaymentDestinationDeleteFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationDeleteFailed} {ex.Message}"
                ));
            }
        }

        [HttpPatch(PathVariables.GetById + "/status/{statusId}")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int id, short statusId)
        {
            try
            {
                var result = await _facade.UpdateStatusAsync(id, statusId);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK,result, PaymentDestinationMessages.SuccessfullyUpdatedPaymentDestinationStatus
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, $"{PaymentDestinationMessages.PaymentDestinationUpdateFailed} {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest,$"{PaymentDestinationMessages.PaymentDestinationUpdateFailed} {ex.Message}"
                ));
            }
        }
    }
}
