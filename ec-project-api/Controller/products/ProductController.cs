using ec_project_api.Constants;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.products;
using ec_project_api.Facades.products;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers {
    [Route(PathVariables.ProductRoot)]
    [ApiController]
    public class ProductController : ControllerBase {
        private readonly ProductFacade _productFacade;

        public ProductController(ProductFacade productFacade) {
            _productFacade = productFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ProductDto>>>> GetAll() {
            try {
                var result = await _productFacade.GetAllAsync();
                return Ok(ResponseData<IEnumerable<ProductDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ProductDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet(PathVariables.GetById)]
        public async Task<ActionResult<ResponseData<ProductDetailDto>>> GetById(int id) {
            try {
                var result = await _productFacade.GetByIdAsync(id);
                return Ok(ResponseData<ProductDetailDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<ProductDetailDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<ProductDetailDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create([FromForm] ProductCreateRequest request) {
            try {
                var result = await _productFacade.CreateAsync(request);
                if (result) {
                    return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, "Sản phẩm đã được tạo thành công"));
                }
                else {
                    return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Failed to create product."));
                }
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
