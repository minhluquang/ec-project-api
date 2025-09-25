using ec_project_api.Constants;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Facades;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
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
            try
            {
                var options = new QueryOptions<Status>();

                if (!string.IsNullOrEmpty(entityType))
                {
                    options.Filter = s => s.EntityType == entityType;
                }

                var result = await _statusFacade.GetAllAsync(options);

                return Ok(ResponseData<IEnumerable<StatusDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<StatusDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<StatusDto>>> GetById(int id, [FromQuery] string? entityType)
        {
            try
            {
                var options = new QueryOptions<Status>();

                if (!string.IsNullOrEmpty(entityType))
                    options.Filter = s => s.EntityType == entityType;

                var result = await _statusFacade.GetByIdAsync(id, options);

                return Ok(ResponseData<StatusDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<StatusDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<StatusDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

    }
}
