using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.locations;
using ec_project_api.Facades.wards;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.wards
{
    [ApiController]
    [Route(PathVariables.WardRoot)]
    public class WardsController : BaseController
    {
        private readonly WardFacade _wardFace;

        public WardsController(WardFacade wardFacade)
        {
            _wardFace = wardFacade;
        }
        
        [HttpGet("province/{provinceId}")]
        public async Task<ActionResult<ResponseData<IEnumerable<WardDto>>>> GetWardsByProvinceId(int provinceId)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _wardFace.GetWardsByProvinceIdAsync(provinceId);
                return ResponseData<IEnumerable<WardDto>>.Success(StatusCodes.Status200OK, result,
                    LocationMessages.GetWardsSuccess);
            });
        }
    }
}
