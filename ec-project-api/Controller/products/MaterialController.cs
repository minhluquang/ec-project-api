using ec_project_api.Constants.messages; // Bạn sẽ cần tạo file MaterialMessages
using ec_project_api.Constants.variables; // Giả sử bạn có PathVariables.MaterialRoot
using ec_project_api.Dtos.request.materials; // Thay đổi namespace cho request DTOs
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.materials; // Thay đổi namespace cho Facade
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Controller.materials // Thay đổi namespace cho Controller
{
    [Route(PathVariables.MaterialRoot)] // Thay đổi route
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly MaterialFacade _materialFacade;

        public MaterialController(MaterialFacade materialFacade)
        {
            _materialFacade = materialFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<MaterialDto>>>> GetAll()
        {
            try
            {
                var result = await _materialFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<MaterialDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<MaterialDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<MaterialDetailDto>>> GetById(short id)
        {
            try
            {
                var result = await _materialFacade.GetByIdAsync(id);
                return Ok(ResponseData<MaterialDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<MaterialDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<MaterialDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] MaterialCreateRequest request)
        {
            try
            {
                var result = await _materialFacade.CreateAsync(request);
                if (result)
                {
                    // Thay đổi thông báo thành công
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, MaterialMessages.SuccessfullyCreatedMaterial));
                }
                else
                {
                    // Thay đổi thông báo thất bại
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create material."));
                }
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

        [HttpPatch(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] MaterialUpdateRequest request)
        {
            try
            {
                var result = await _materialFacade.UpdateAsync(id, request);
                // Thay đổi thông báo thành công
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, MaterialMessages.SuccessfullyUpdatedMaterial));
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

        [HttpDelete(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            try
            {
                var result = await _materialFacade.DeleteAsync(id);
                // Thay đổi thông báo thành công
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, MaterialMessages.SuccessfullyDeletedMaterial));
            }
            catch (KeyNotFoundException ex)
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