using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.categories;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using ec_project_api.Facades.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.CategoryRoot)]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly CategoryFacade _categoryFacade;

        public CategoryController(CategoryFacade categoryFacade)
        {
            _categoryFacade = categoryFacade;
        }

        [HttpGet("all")]
        [Authorize(Policy = "Category.GetAll")]
        public async Task<ActionResult<ResponseData<PagedResult<CategoryDetailDto>>>> GetAll([FromQuery] CategoryFilter filter)
        {
            return await ExecuteAsync(async () =>
            {
                var categories = await _categoryFacade.GetAllPagedAsync(filter);
                return ResponseData<PagedResult<CategoryDetailDto>>.Success(StatusCodes.Status200OK, categories, CategoryMessages.CategoryRetrievedSuccessfully);
            });
        }

        [HttpGet(PathVariables.GetById)]
        [Authorize(Policy = "Category.GetById")]
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
        [Authorize(Policy = "PurchaseOrder.GetAll")]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromForm] CategoryCreateRequest request)
        {
            try
            {
                var result = await _categoryFacade.CreateAsync(request);
                if (result)
                {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, CategoryMessages.SuccessfullyCreatedCategory));
                }
                else
                {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create category."));
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
        [Authorize(Policy = "Category.Update")]
        public async Task<ActionResult<ResponseData<bool>>> Update(short id, [FromForm] CategoryUpdateRequest request)
        {
            try
            {
                await _categoryFacade.UpdateAsync(id, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, CategoryMessages.SuccessfullyUpdatedCategory));
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
        [Authorize(Policy = "Category.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> Delete(short id)
        {
            try
            {
                await _categoryFacade.DeleteAsync(id);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, CategoryMessages.SuccessfullyDeletedCategory));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
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

        [HttpGet("hierarchy")]
        [Authorize(Policy = "Category.GetHierarchy")]
        public async Task<ActionResult<ResponseData<IEnumerable<CategoryDto>>>> GetHierarchyForSelection()
        {
            return await ExecuteAsync(async () =>
            {
                var categories = await _categoryFacade.GetHierarchyForSelectionAsync();
                return ResponseData<IEnumerable<CategoryDto>>.Success(
                    StatusCodes.Status200OK,
                    categories,
                    CategoryMessages.CategoryRetrievedSuccessfully
                );
            });
        }
    }
}