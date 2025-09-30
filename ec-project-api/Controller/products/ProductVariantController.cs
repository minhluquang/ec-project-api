using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.products {
    [ApiController]
    [Route(PathVariables.ProductRoot + "/{productId}/variants")]
    public class ProductVariantController : ControllerBase {
        private readonly ProductVariantFacade _productVarianFacade;

        public ProductVariantController(ProductVariantFacade productVarianFacade) {
            _productVarianFacade = productVarianFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductVariantDto>>>> getGetAllByProductId(int productId) {
            try {
                var result = await _productVarianFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ProductVariantDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ProductVariantDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
