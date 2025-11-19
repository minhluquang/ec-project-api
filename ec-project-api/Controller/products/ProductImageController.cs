using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "ProductImage.GetAll")]
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
        [Authorize(Policy = "ProductImage.Upload")]
        public async Task<ActionResult<ResponseData<bool>>> UploadSingleProductImage(int productId, [FromForm] ProductImageRequest request) {
            try {
                await _productImageFacade.UploadSingleProductImageAsync(productId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, ProductMessages.ProductImageUploadSuccessully));

            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch]
        [Authorize(Policy = "ProductImage.UpdateOrder")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateImageDisplayOrder(int productId, [FromBody] List<ProductUpdateImageDisplayOrderRequest> request) {
            try {
                await _productImageFacade.UpdateImageDisplayOrderAsync(productId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ProductMessages.ProductImageDisplayOrderUpdateSuccessully));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete("{productImageId}")]
        [Authorize(Policy = "ProductImage.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> DeleteProductImage(int productId, int productImageId) {
            try {
                await _productImageFacade.DeleteProductImageAsync(productId, productImageId);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ProductMessages.SuccessfullyDeletedProductImage));
            }
            catch (KeyNotFoundException knfEx) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, knfEx.Message));
            }
            catch (InvalidOperationException ioEx) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ioEx.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}