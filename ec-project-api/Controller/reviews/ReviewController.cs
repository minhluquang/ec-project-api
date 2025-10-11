using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.request.reviews;
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

        [HttpPatch("/{reviewId}/status")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int reviewId, ReviewUpdateStatusRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _reviewFacade.UpdateStatus(reviewId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewMessages.SuccessfullyUpdatedReview));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ReviewDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost("{orderItemId}")]
        public async Task<ActionResult<ResponseData<bool>>> Create(int orderItemId, [FromForm] ReviewCreateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _reviewFacade.CreateAsync(orderItemId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewMessages.SuccessfullyCreatedReview));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch("{reviewId}")]
        public async Task<ActionResult<ResponseData<bool>>> Update(int reviewId, ReviewUpdateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _reviewFacade.UpdateAsync(reviewId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewMessages.SuccessfullyUpdatedReview));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet("{reviewId}")]
        public async Task<ActionResult<ResponseData<ReviewDto>>> GetById(int reviewId) {
            try {
                var result = await _reviewFacade.GetByIdAsync(reviewId);
                return Ok(ResponseData<ReviewDto>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<ReviewDto>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<ReviewDto>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}