using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.products
{
    [Route(PathVariables.SizeRoot)]
    [ApiController]
    public class SizeController : BaseController
    {
        private readonly SizeFacade _sizeFacade;

        public SizeController(SizeFacade sizeFacade)
        {
            _sizeFacade = sizeFacade;
        }

        [HttpGet("options")]
        public async Task<ActionResult<ResponseData<IEnumerable<SizeDto>>>> GetSizeOptionsAsync()
        {
            try
            {
                var result = await _sizeFacade.GetSizeOptionsAsync();
                return Ok(ResponseData<IEnumerable<SizeDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<SizeDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<ResponseData<PagedResult<SizeDetailDto>>>> GetAll([FromQuery] SizeFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var users = await _sizeFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<SizeDetailDto>>.Success(StatusCodes.Status200OK, users, SizeMessages.SizeRetrievedSuccessfully);
            });
        }


        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<SizeDetailDto>>> GetById(short id)
        {
            try
            {
                var result = await _sizeFacade.GetByIdAsync(id);
                return Ok(ResponseData<SizeDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<SizeDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<SizeDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] SizeCreateRequest request)
        {
            try
            {
                var result = await _sizeFacade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, SizeMessages.SuccessfullyCreatedSize));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create size."));
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
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] SizeUpdateRequest request)
        {
            try
            {
                var result = await _sizeFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, SizeMessages.SuccessfullyUpdatedSize));
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
                var result = await _sizeFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, SizeMessages.SuccessfullyDeletedSize));
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
