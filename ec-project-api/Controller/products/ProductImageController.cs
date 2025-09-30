using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.products {
    [ApiController]
    [Route(PathVariables.ProductImageRoot)]
    public class ProductImageController : ControllerBase {
        private readonly ProductImageFacade _productImageFacade;

        public ProductImageController(ProductImageFacade productImageFacade) {
            _productImageFacade = productImageFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductImageDetailDto>>>> GetAllByProductId(int productId) {
            try {
                var result = await _productImageFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ProductImageDetailDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ProductImageDetailDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> UploadSingleProductImage(int productId, [FromForm] ProductImageRequest request) {
            try {
                var result = await _productImageFacade.UploadSingleProductImageAsync(productId, request);
                if (result) {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, "Hình ảnh sản phẩm đã được tải lên thành công"));
                }
                else {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to upload product image."));
                }
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}
