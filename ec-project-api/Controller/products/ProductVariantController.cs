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
    [Route(PathVariables.ProductVariantRoot)]
    public class ProductVariantController : ControllerBase {
        private readonly ProductVariantFacade _productVarianFacade;

        public ProductVariantController(ProductVariantFacade productVarianFacade) {
            _productVarianFacade = productVarianFacade;
        }

        [HttpGet]
        [Authorize(Policy = "ProductVariant.GetAll")]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductVariantDetailDto>>>> getGetAllByProductId(int productId) {
            try {
                var result = await _productVarianFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ProductVariantDetailDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ProductVariantDetailDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Policy = "ProductVariant.Create")]
        public async Task<ActionResult<ResponseData<bool>>> Create(int productId, [FromBody] ProductVariantCreateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _productVarianFacade.CreateAsync(productId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ProductMessages.SuccessfullyCreatedProductVariant));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch("{productVariantId}")]
        [Authorize(Policy = "ProductVariant.Update")]
        public async Task<ActionResult<ResponseData<bool>>> Update(int productId, int productVariantId, [FromBody] ProductVariantUpdateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _productVarianFacade.UpdateAsync(productId, productVariantId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ProductMessages.SuccessfullyUpdatedProductVariant));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpDelete("{productVariantId}")]
        [Authorize(Policy = "ProductVariant.Delete")]
        public async Task<ActionResult<ResponseData<bool>>> Delete(int productId, int productVariantId) {
            try {
                await _productVarianFacade.DeleteAsync(productId, productVariantId);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ProductMessages.SuccessfullyDeletedProductVariant));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex) {
                return BadRequest(ResponseData<bool>.Error( StatusCodes.Status400BadRequest,ex.Message));
            }
            catch (InvalidOperationException ex) {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}