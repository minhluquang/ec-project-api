using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.products
{
    [Route(PathVariables.ColorRoot)]
    [ApiController]
    public class ColorController : BaseController
    {
        private readonly ColorFacade _colorFacade;

        public ColorController(ColorFacade colorFacade)
        {
            _colorFacade = colorFacade;
        }

        //[HttpGet]
        //public async Task<ActionResult<ResponseData<IEnumerable<ColorDto>>>> GetAll()
        //{
        //    try
        //    {
        //        var result = await _colorFacade.GetAllAsync();
        //        return Ok(ResponseData<IEnumerable<ColorDto>>.Success(StatusCodes.Status200OK, result));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ResponseData<IEnumerable<ColorDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
        //    }
        //}
        [HttpGet]
        [Authorize(Policy = "Color.GetAll")]
        public async Task<ActionResult<ResponseData<PagedResult<ColorDetailDto>>>> GetAll([FromQuery] ColorFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var users = await _colorFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<ColorDetailDto>>.Success(StatusCodes.Status200OK, users, ColorMessages.ColorRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        [Authorize(Policy = "Shipping.GetById")]
        public async Task<ActionResult<ResponseData<ColorDetailDto>>> GetById(short id)
        {
            try
            {
                var result = await _colorFacade.GetByIdAsync(id);
                return Ok(ResponseData<ColorDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<ColorDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<ColorDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Policy = "Color.Create")]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] ColorCreateRequest request)
        {
            try
            {
                var result = await _colorFacade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, ColorMessages.SuccessfullyCreatedColor));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create color."));
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
        [Authorize(Policy = "Color.Update")]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] ColorUpdateRequest request)
        {
            try
            {
                var result = await _colorFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, ColorMessages.SuccessfullyUpdatedColor));
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
        [Authorize(Policy = "Shipping.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            try
            {
                var result = await _colorFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, ColorMessages.SuccessfullyDeletedColor));
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
