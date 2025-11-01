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

namespace ec_project_api.Controller.products;

[Route(PathVariables.ProductRoot)]
[ApiController]
public class ProductController : BaseController
{
    private readonly ProductFacade _productFacade;

    public ProductController(ProductFacade productFacade)
    {
        _productFacade = productFacade;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseData<PagedResult<ProductDto>>>> GetAll([FromQuery] ProductFilter filter)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _productFacade.GetAllPagedAsync(filter);
            return ResponseData<PagedResult<ProductDto>>.Success(StatusCodes.Status200OK, result,
                ProductMessages.ProductRetrievedSuccessfully);
        });
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseData<ProductDetailDto>>> GetBySlug(string slug)
    {
        try
        {
            var result = await _productFacade.GetBySlugAsync(slug);
            return Ok(ResponseData<ProductDetailDto>.Success(StatusCodes.Status200OK, result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ResponseData<ProductDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ResponseData<ProductDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
        }
    }
    
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseData<IEnumerable<ProductDto>>>> Search([FromQuery] string search)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _productFacade.SearchTop5Async(search);
            return ResponseData<IEnumerable<ProductDto>>.Success(StatusCodes.Status200OK, result,
                ProductMessages.ProductRetrievedSuccessfully);
        });
    }
    

    [HttpPost]
    public async Task<ActionResult<ResponseData<bool>>> Create([FromForm] ProductCreateRequest request)
    {
        try
        {
            var result = await _productFacade.CreateAsync(request);
            if (result)
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true,
                    ProductMessages.SuccessfullyCreatedProduct));

            return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create product."));
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
    public async Task<ActionResult<ResponseData<bool>>> Update(int id, ProductUpdateRequest request)
    {
        try
        {
            await _productFacade.UpdateAsync(id, request);
            return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true,
                ProductMessages.SuccessfullyUpdatedProduct));
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

    [HttpGet("catelog")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseData<PagedResultWithCategory<ProductDto>>>> GetAllByCategorySlugAsync(
        [FromQuery] ProductCategorySlugFilter filter)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _productFacade.GetAllByCategorySlugPagedAsync(filter);
            return ResponseData<PagedResultWithCategory<ProductDto>>.Success(StatusCodes.Status200OK, result,
                ProductMessages.ProductRetrievedSuccessfully);
        });
    }
     
    [HttpDelete(PathVariables.GetById)]
    public async Task<ActionResult<ResponseData<bool>>> Delete(int id)
    {
        try
        {
            await _productFacade.DeleteAsync(id);
            return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true,
                ProductMessages.SuccessfullyDeletedProduct));
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

    [HttpGet("form-meta")]
    public async Task<ActionResult<ResponseData<ProductFormMetaDto>>> GetProductFormMeta()
    {
        try
        {
            var result = await _productFacade.GetProductFormMetaAsync();
            return Ok(ResponseData<ProductFormMetaDto>.Success(StatusCodes.Status200OK, result));
        }
        catch (Exception ex)
        {
            return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
        }
    }

    [HttpGet("catelog/filter-options")]
    [AllowAnonymous]
    public async Task<ActionResult<ResponseData<ProductFilterOptionDto>>> GetFilterOptions([FromQuery] string? categorySlug, [FromQuery] string? search)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _productFacade.GetFilterOptionsByCategorySlugAsync(categorySlug, search);
            return ResponseData<ProductFilterOptionDto>.Success(StatusCodes.Status200OK, result,
                ProductMessages.ProductFilterOptionsRetrievedSuccessfully);
        });
    }

}
