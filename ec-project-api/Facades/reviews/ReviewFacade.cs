using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response.products;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.reviews;

namespace ec_project_api.Facades.reviews {
    public class ReviewFacade {
        private readonly IReviewService _reviewService;
        private readonly IOrderItemService _orderItemService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ReviewFacade(IReviewService reviewService, IOrderItemService orderItemService, IStatusService statusService, IMapper mapper) {
            _reviewService = reviewService;
            _orderItemService = orderItemService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllByProductIdAsync(int productId) {
            var reviews = await _reviewService.GetAllByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<bool> UpdateStatus(int reviewId, ReviewUpdateStatusRequest request) {
            var review = await _reviewService.GetByIdAsync(reviewId) ??
                throw new KeyNotFoundException(ReviewMessages.ReviewNotFound);

            var existingStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Review && s.StatusId == request.StatusId) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            review.Status = existingStatus;
            return await _reviewService.UpdateAsync(review);
        }

        public async Task<bool> CreateAsync(int orderItemId, ReviewCreateRequest request) {
            if (request?.Images?.Count > 5)
                throw new ArgumentException(ReviewMessages.TooManyReviewImages);

            var orderItem = await _orderItemService.GetByIdAsync(orderItemId) ??
                throw new KeyNotFoundException(OrderMessages.OrderItemNotFound);


            var approvedStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Review && s.Name == StatusVariables.Approved) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var review = _mapper.Map<Review>(request);
            review.StatusId = approvedStatus.StatusId;
            review.OrderItemId = orderItemId;

            return await _reviewService.CreateReviewAndUploadReviewImagesAsync(review, request?.Images);
        }

        public async Task<bool> UpdateAsync(int reviewId, ReviewUpdateRequest request) {
            var review = await _reviewService.GetByIdAsync(reviewId) ??
                throw new KeyNotFoundException(ReviewMessages.ReviewNotFound);

            if (review.IsEdited)
                throw new InvalidOperationException(ReviewMessages.ReviewUpdateAlreadyEdited);

            _mapper.Map(request, review);
            review.IsEdited = true;
            review.UpdatedAt = DateTime.UtcNow;

            return await _reviewService.UpdateAsync(review);
        }

        public async Task<ReviewDto> GetByIdAsync(int reviewId) {
            var review = await _reviewService.GetByIdAsync(reviewId);
            return _mapper.Map<ReviewDto>(review);
        }
    }
}