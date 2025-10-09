using ec_project_api.Constants.messages; // Bạn cần tạo file CategoryMessages
using ec_project_api.Constants.variables; // Giả sử có PathVariables.CategoryRoot
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.categories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Controller.categories
{
    [Route(PathVariables.CategoryRoot)]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryFacade _categoryFacade;

        public CategoryController(CategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<CategoryDto>>>> GetAll()
        {
            try
            {
                var result = await _categoryFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<CategoryDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<CategoryDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<CategoryDetailDto>>> GetById(short id)
        {
            try
            {
                var result = await _categoryFacade.GetByIdAsync(id);
                return Ok(ResponseData<CategoryDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ResponseData<CategoryDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<CategoryDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromBody] CategoryCreateRequest request)
        {
            try
            {
                var result = await _categoryFacade.CreateAsync(request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, result, CategoryMessages.SuccessfullyCreatedCategory));
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
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromBody] CategoryUpdateRequest request)
        {
            try
            {
                var result = await _categoryFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, CategoryMessages.SuccessfullyUpdatedCategory));
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
                var result = await _categoryFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result, CategoryMessages.SuccessfullyDeletedCategory));
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