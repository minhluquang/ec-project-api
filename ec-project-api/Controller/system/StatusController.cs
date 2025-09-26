using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Facades;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.StatusRoot)]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly StatusFacade _statusFacade;

        public StatusController(StatusFacade statusFacade)
        {
            _statusFacade = statusFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<StatusDto>>>> GetAll([FromQuery] string? entityType)
        {
            return await HandleRequestAsync(async () =>
            {
                var result = await _statusFacade.GetAllAsync(entityType);
                return ResponseData<IEnumerable<StatusDto>>.Success(StatusCodes.Status200OK, result, StatusMessages.StatusListRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<StatusDto>>> GetById(int id, [FromQuery] string? entityType)
        {
            return await HandleRequestAsync(async () =>
            {
                var result = await _statusFacade.GetByIdAsync(id, entityType);
                return ResponseData<StatusDto>.Success(StatusCodes.Status200OK, result);
            });
        }

        private async Task<ActionResult<ResponseData<T>>> HandleRequestAsync<T>(Func<Task<ResponseData<T>>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<T>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseData<T>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<T>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
