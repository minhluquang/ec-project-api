using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.locations;
using Microsoft.AspNetCore.Mvc;
using ec_project_api.Facades.provinces;

namespace ec_project_api.Controller.provinces
{
    [ApiController]
    [Route(PathVariables.ProvinceRoot)]
    public class ProvincesController : BaseController
    {
        private readonly ProvinceFacade _provinceFacade;

        public ProvincesController(ProvinceFacade provinceFacade)
        {
            _provinceFacade = provinceFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProvinceDto>>>> GetAllProvinces()
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _provinceFacade.GetAllProvincesAsync();
                return ResponseData<IEnumerable<ProvinceDto>>.Success(StatusCodes.Status200OK, result,
                    LocationMessages.GetProvincesSuccess);
            });
        }
    }
}

