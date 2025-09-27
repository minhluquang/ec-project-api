using ec_project_api.Constants;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.products
{
    [ApiController]
    [Route(PathVariables.ProductRoot + "/{productId}/images")]
    public class ProductImageController : ControllerBase
    {
        private readonly ProductImageFacade _productImageFacade;

        public ProductImageController(ProductImageFacade productImageFacade)
        {
            _productImageFacade = productImageFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductImageDetailDto>>>> GetAllByProductId(int productId)
        {
            try
            {
                var result = await _productImageFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ProductImageDetailDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseData<IEnumerable<ProductImageDetailDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
