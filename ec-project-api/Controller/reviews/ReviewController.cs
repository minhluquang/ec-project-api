using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Facades.reviews;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.reviews {
    [ApiController]
    [Route(PathVariables.ReviewRoot)]
    public class ReviewController : ControllerBase {
        private readonly ReviewFacade _reviewFacade;

        public ReviewController(ReviewFacade reviewFacade) {
            _reviewFacade = reviewFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ReviewDto>>>> GetAllByProductId(int productId) {
            try {
                var result = await _reviewFacade.GetAllByProductIdAsync(productId);
                return Ok(ResponseData<IEnumerable<ReviewDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ReviewDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}