using ec_project_api.Constants.messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.reviewreports;
using ec_project_api.Facades.ReviewReports;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controller.ReviewReports {
    [ApiController]
    [Route(PathVariables.ReviewReportRoot)]
    public class ReviewReportController : ControllerBase {
        private readonly ReviewReportFacade _reviewReportFacade;

        public ReviewReportController(ReviewReportFacade reviewReportFacade) {
            _reviewReportFacade = reviewReportFacade;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<ReviewReportDto>>>> GetAllByReviewId(int reviewId) {
            try {
                var result = await _reviewReportFacade.GetAllByReviewIdAsync(reviewId);
                return Ok(ResponseData<IEnumerable<ReviewReportDto>>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<IEnumerable<ReviewReportDto>>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<IEnumerable<ReviewReportDto>>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseData<bool>>> Create(int reviewId, [FromQuery] int userId, [FromBody] ReviewReportCreateRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _reviewReportFacade.CreateAsync(reviewId, userId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status201Created, true, ReviewReportMessages.SuccessfullyCreatedReviewReport));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (InvalidOperationException ex) {
                return Conflict(ResponseData<bool>.Error(StatusCodes.Status409Conflict, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }

        [HttpPatch("{reviewReportId}/status")]
        public async Task<ActionResult<ResponseData<bool>>> UpdateStatus(int reviewReportId, [FromBody] ReviewReportUpdateStatusRequest request) {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }

            try {
                await _reviewReportFacade.UpdateStatusAsync(reviewReportId, request);
                return Ok(ResponseData<bool>.Success(StatusCodes.Status200OK, true, ReviewReportMessages.SuccessfullyUpdatedReviewReport));
            }
            catch (KeyNotFoundException ex) {
                return NotFound(ResponseData<bool>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (Exception ex) {
                return BadRequest(ResponseData<bool>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
        }
    }
}