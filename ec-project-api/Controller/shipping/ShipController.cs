using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.shipping;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.shipping;
using ec_project_api.Facades.Ships;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.ShipRoot)]
    [ApiController]
    public class ShipController : BaseController
    {
        private readonly ShipFacade _shipFacade;

        public ShipController(ShipFacade shipFacade)
        {
            _shipFacade = shipFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<ShipDto>>>> GetAll([FromQuery] ShipFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _shipFacade.GetAllPagedAsync(filter ?? new ShipFilter());
                return ResponseData<PagedResult<ShipDto>>.Success(StatusCodes.Status200OK, result);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<ShipDto>>> GetByIdAsync(short id)
        {
            try
            {
                var result = await _shipFacade.GetByIdAsync(id);
                return Ok(ResponseData<ShipDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<ShipDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<ShipDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> CreateAsync([FromBody] ShipCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _shipFacade.CreateAsync(request);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, ShipMessages.CreateSuccess))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ShipMessages.CreateFailed));
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

        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> UpdateAsync(short id, [FromBody] ShipUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _shipFacade.UpdateAsync(id, request);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ShipMessages.UpdateSuccess)) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ShipMessages.UpdateFailed));
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

        // [HttpDelete(PathVariables.GetById)]
        // public async Task<ActionResult<ResponseData<bool>>> DeleteAsync(byte id)
        // {
        //     try
        //     {
        //         var result = await _shipFacade.DeleteAsync(id);
        //         return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ShipMessages.DeleteSuccess)) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ShipMessages.DeleteFailed));
        //     }
        //     catch (InvalidOperationException ex)
        //     {
        //         return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
        //     }
        // }

        [HttpPatch(PathVariables.GetById+"/activate")]
        public async Task<ActionResult<ResponseData<bool>>> SetActiveStatus(short id)
        {
            try
            {
                var result = await _shipFacade.SetActiveStatusAsync(id);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ShipMessages.UpdateStatusSuccess)) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ShipMessages.UpdateStatusFailed));
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
    }
}
