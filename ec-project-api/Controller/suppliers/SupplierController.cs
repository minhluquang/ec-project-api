using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Facades.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.SupplierRoot)]
    [ApiController]
    public class SupplierController : BaseController
    {
        private readonly SupplierFacade _supplierFacade;

        public SupplierController(SupplierFacade supplierFacade)
        {
            _supplierFacade = supplierFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<SupplierDto>>>> GetAllAsync([FromQuery] SupplierFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _supplierFacade.GetAllPagedAsync(filter ?? new SupplierFilter());
                return ResponseData<PagedResult<SupplierDto>>.Success(StatusCodes.Status200OK, result);
            });
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<SupplierDto>>> GetByIdAsync(int id)
        {
            try
            {
                var result = await _supplierFacade.GetByIdAsync(id);
                if (result == null)
                    return NotFound(ResponseData<SupplierDto>.Error(StatusCodes.Status404NotFound, SupplierMessages.SupplierNotFound));

                return Ok(ResponseData<SupplierDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<SupplierDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<SupplierDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> CreateAsync([FromBody] SupplierCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _supplierFacade.CreateAsync(request);
                return result
                    ? Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, SupplierMessages.CreateSuccess))
                    : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, SupplierMessages.CreateFailed));
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
        public async Task<ActionResult<ResponseData<bool>>> UpdateAsync(int id, [FromBody] SupplierUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            try
            {
                var result = await _supplierFacade.UpdateAsync(id, request);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, SupplierMessages.UpdateSuccess)) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, SupplierMessages.UpdateFailed));
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

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> DeleteAsync(int id)
        {
            try
            {
                var result = await _supplierFacade.DeleteAsync(id);
                return result ? Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, SupplierMessages.DeleteSuccess)) : BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, SupplierMessages.DeleteFailed));
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
    }
}
