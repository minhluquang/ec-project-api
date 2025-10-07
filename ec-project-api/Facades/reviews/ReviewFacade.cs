using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Services;

namespace ec_project_api.Facades.reviews {
    public class ReviewFacade {
        private readonly IReviewService _reviewService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ReviewFacade(IReviewService reviewService, IStatusService statusService, IMapper mapper) {
            _reviewService = reviewService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllByProductIdAsync(int productId) {
            var reviews = await _reviewService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<bool> UpdateStatus(int reviewId, ReviewUpdateStatusRequest request) {
            var review = await _reviewService.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException(ReviewMessages.ReviewNotFound);

            var existingStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Review && s.StatusId == request.StatusId);
            if (existingStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            review.Status = existingStatus;
            return await _reviewService.UpdateAsync(review);
        }
    }
}