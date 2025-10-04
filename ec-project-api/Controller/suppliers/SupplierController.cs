using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Facades.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers.Suppliers
{
    [ApiController]
    [Route(PathVariables.SupplierRoot)]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierFacade _supplierFacade;

        public SupplierController(SupplierFacade supplierFacade)
        {
            _supplierFacade = supplierFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<SupplierDto>>>> GetAll()
            => Ok(await _supplierFacade.GetAllAsync());

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<SupplierDto>>> GetById(int id)
            => Ok(await _supplierFacade.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<ResponseData<SupplierDto>>> Create([FromBody] SupplierCreateRequest request)
            => Ok(await _supplierFacade.CreateAsync(request));

        [HttpPut(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(int id, [FromBody] SupplierUpdateRequest request)
             => Ok(await _supplierFacade.UpdateAsync(id, request));

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
            => Ok(await _supplierFacade.DeleteAsync(id));
        [HttpGet("paged")]
        public async Task<ActionResult<ResponseData<PagedResponse<SupplierDto>>>> GetPaged([FromQuery] SupplierQueryRequest filter)
            => Ok(await _supplierFacade.GetPagedAsync(filter));
    }
}
