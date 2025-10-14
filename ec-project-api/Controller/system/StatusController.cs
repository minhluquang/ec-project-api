using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Facades;
using ec_project_api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.StatusRoot)]
    public class StatusController : BaseController
    {
        private readonly StatusFacade _statusFacade;

        public StatusController(StatusFacade statusFacade)
        {
            _statusFacade = statusFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<StatusDto>>>> GetAll([FromQuery] string? entityType)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _statusFacade.GetAllAsync(entityType);
                return ResponseData<IEnumerable<StatusDto>>.Success(StatusCodes.Status200OK, result, StatusMessages.StatusListRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<StatusDto>>> GetById(short id, [FromQuery] string? entityType)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _statusFacade.GetByIdAsync(id, entityType);
                return ResponseData<StatusDto>.Success(StatusCodes.Status200OK, result);
            });
        }
    }
}
