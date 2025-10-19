using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.homepage;
using ec_project_api.Dtos.response;
using ec_project_api.Facades.Homepage;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [Route(PathVariables.Homepage)]
    [ApiController]
    public class HomepageController : ControllerBase
    {
        private readonly HomepageFacade _homepageFacade;

        public HomepageController(HomepageFacade homepageFacade)
        {
            _homepageFacade = homepageFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<HomepageDto>>> GetHomepageDataAsync()
        {
            try
            {
                var result = await _homepageFacade.GetHomepageDataAsync();
                return Ok(ResponseData<HomepageDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<HomepageDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<HomepageDto>.Error(StatusCodes.Status400BadRequest, $"{HomepageMessages.ErrorLoadingHomepage}: {ex.Message}"));
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ResponseData<List<CategoryHomePageDto>>>> GetCategoriesAsync()
        {
            try
            {
                var result = await _homepageFacade.GetCategoriesAsync();
                return Ok(ResponseData<List<CategoryHomePageDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<List<CategoryHomePageDto>>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<List<CategoryHomePageDto>>.Error(StatusCodes.Status400BadRequest, $"{HomepageMessages.ErrorLoadingCategories}: {ex.Message}"));
            }
        }

        [HttpGet("bestselling")]
        public async Task<ActionResult<ResponseData<List<ProductSummaryDto>>>> GetBestSellingProductsAsync()
        {
            try
            {
                var result = await _homepageFacade.GetBestSellingProductsAsync();
                return Ok(ResponseData<List<ProductSummaryDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<List<ProductSummaryDto>>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<List<ProductSummaryDto>>.Error(StatusCodes.Status400BadRequest, $"{HomepageMessages.ErrorLoadingBestSelling}: {ex.Message}"));
            }
        }

        [HttpGet("onsale")]
        public async Task<ActionResult<ResponseData<List<ProductSummaryDto>>>> GetOnSaleProductsAsync()
        {
            try
            {
                var result = await _homepageFacade.GetOnSaleProductsAsync();
                return Ok(ResponseData<List<ProductSummaryDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ResponseData<List<ProductSummaryDto>>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<List<ProductSummaryDto>>.Error(StatusCodes.Status400BadRequest, $"{HomepageMessages.ErrorLoadingOnSale}: {ex.Message}"));
            }
        }
    }
}
