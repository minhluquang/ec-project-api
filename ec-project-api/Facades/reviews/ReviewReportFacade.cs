using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response.reviewreports;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.reviews;

namespace ec_project_api.Facades.ReviewReports {
    public class ReviewReportFacade {
        private readonly IReviewReportService _reviewReportService;
        private readonly IReviewService _reviewService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ReviewReportFacade(
            IReviewReportService reviewReportService,
            IReviewService reviewService,
            IStatusService statusService,
            IMapper mapper) {
            _reviewReportService = reviewReportService;
            _reviewService = reviewService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewReportDto>> GetAllByReviewIdAsync(int reviewId) {
            var review = await _reviewService.GetByIdAsync(reviewId) ??
                throw new KeyNotFoundException(ReviewReportMessages.ReviewNotFound);

            var reviewReports = await _reviewReportService.GetAllByReviewIdAsync(reviewId);
            return _mapper.Map<IEnumerable<ReviewReportDto>>(reviewReports);
        }

        public async Task<bool> CreateAsync(int reviewId, int userId, ReviewReportCreateRequest request) {
            var review = await _reviewService.GetByIdAsync(reviewId) ??
                throw new KeyNotFoundException(ReviewReportMessages.ReviewNotFound);

            if (review.OrderItem?.Order?.UserId == userId)
                throw new InvalidOperationException(ReviewReportMessages.CannotReportOwnReview);

            var hasReported = await _reviewReportService.HasUserReportedReviewAsync(reviewId, userId);
            if (hasReported)
                throw new InvalidOperationException(ReviewReportMessages.ReviewReportAlreadyExists);

            var pendingStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.ReviewReport && s.Name == StatusVariables.Pending
            ) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var reviewReport = _mapper.Map<ReviewReport>(request);
            reviewReport.ReviewId = reviewId;
            reviewReport.UserId = userId;
            reviewReport.StatusId = pendingStatus.StatusId;

            return await _reviewReportService.CreateAsync(reviewReport);
        }

        public async Task<bool> UpdateStatusAsync(int reviewReportId, ReviewReportUpdateStatusRequest request) {
            var reviewReport = await _reviewReportService.GetByIdAsync(reviewReportId) ??
                throw new KeyNotFoundException(ReviewReportMessages.ReviewReportNotFound);

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.ReviewReport)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            reviewReport.StatusId = request.StatusId;
            reviewReport.UpdatedAt = DateTime.UtcNow;

            if (status.Name == StatusVariables.Handled) {
                var review = await _reviewService.GetByIdAsync(reviewReport.ReviewId) ??
                    throw new KeyNotFoundException(ReviewMessages.ReviewNotFound);

                var rejectedStatus = await _statusService.FirstOrDefaultAsync(
                    s => s.EntityType == EntityVariables.Review && s.Name == StatusVariables.Rejected
                ) ??
                    throw new InvalidOperationException(StatusMessages.StatusNotFound);

                review.StatusId = rejectedStatus.StatusId;
                review.UpdatedAt = DateTime.UtcNow;

                await _reviewService.UpdateAsync(review);
            }

            return await _reviewReportService.UpdateAsync(reviewReport);
        }
    }
}