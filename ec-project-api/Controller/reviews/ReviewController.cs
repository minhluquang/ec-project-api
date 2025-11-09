using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Controllers.Base;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Facades.reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.reviews {
    [ApiController]
    [Route(PathVariables.ReviewRoot)]
    public class ReviewController : BaseController {
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

        [HttpGet("product/{productId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseData<PagedResult<ReviewDto>>>> GetPagedByProductId(int productId, [FromQuery] ReviewFilter filter) {
            return await ExecuteAsync(async () =>
            {
                var pagedResult = await _reviewFacade.GetPagedByProductIdAsync(productId, filter);
                return ResponseData<PagedResult<ReviewDto>>.Success(StatusCodes.Status200OK, pagedResult, ReviewMessages.ReviewsRetrievedSuccessfully);
            });
        }

        [HttpPatch("{reviewId}/toggle-visibility")]
        public async Task<ActionResult<ResponseData<bool>>> ToggleReviewStatus(int reviewId) {
            try {
                await _reviewFacade.ToggleReviewStatus(reviewId);
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
                var currentUser = User;
                await _reviewFacade.CreateAsync(currentUser, orderItemId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewMessages.SuccessfullyCreatedReview));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch("{reviewId}")]
        public async Task<ActionResult<ResponseData<bool>>> Update(int reviewId, [FromForm] ReviewUpdateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                var currentUser = User;
                await _reviewFacade.UpdateAsync(currentUser, reviewId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewMessages.SuccessfullyUpdatedReview));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpGet("{reviewId:int}")]
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