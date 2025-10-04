using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
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
        public async Task<ActionResult<ResponseData<IEnumerable<ProductVariantDto>>>> getGetAllByProductId(int productId) {
            try {
                var result = await _productVarianFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ProductVariantDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ProductVariantDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create(int productId, [FromBody] ProductVariantCreateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                var result = await _productVarianFacade.CreateAsync(productId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch("{productVariantId}")]
        public async Task<ActionResult<ResponseData<bool>>> Update(int productId, int productVariantId, [FromBody] ProductVariantUpdateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                var result = await _productVarianFacade.UpdateAsync(productId, productVariantId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}