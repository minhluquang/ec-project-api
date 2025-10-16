using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.request.reviews;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.order_items;
using ec_project_api.Services.reviews;
using System.Linq.Expressions;

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

        private static Expression<Func<Review, bool>> BuildReviewFilter(int productId, ReviewFilter filter)
        {
            int? searchReviewId = null;

            if (!string.IsNullOrEmpty(filter.Search) &&
                filter.Search.StartsWith("REV", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(filter.Search.Substring(3), out var parsed))
                    searchReviewId = parsed;
            }

            return r =>
                (r.OrderItem != null &&
                 r.OrderItem.ProductVariant != null &&
                 r.OrderItem.ProductVariant.ProductId == productId) &&

                (string.IsNullOrEmpty(filter.StatusName) ||
                 (r.Status != null && r.Status.Name == filter.StatusName && r.Status.EntityType == EntityVariables.Review)) &&

                (string.IsNullOrEmpty(filter.Search) ||
                 (r.Comment != null && r.Comment.Contains(filter.Search)) ||
                 (r.OrderItem != null && r.OrderItem.Order != null && r.OrderItem.Order.User != null && r.OrderItem.Order.User.Username.Contains(filter.Search)) ||
                 r.ReviewId.ToString().Contains(filter.Search) ||
                 (searchReviewId.HasValue && r.ReviewId == searchReviewId.Value)
                ) &&

                (!filter.Rating.HasValue || r.Rating == filter.Rating.Value);
        }


        public async Task<PagedResult<ReviewDto>> GetPagedByProductIdAsync(int productId, ReviewFilter filter) {
            var options = new QueryOptions<Review>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            options.Filter = BuildReviewFilter(productId, filter);
            var pagedResult = await _reviewService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<ReviewDto>>(pagedResult.Items);

            return new PagedResult<ReviewDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<bool> HideReview(int reviewId) {
            var review = await _reviewService.GetByIdAsync(reviewId) ??
                throw new KeyNotFoundException(ReviewMessages.ReviewNotFound);

            var hiddenStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Review && s.Name == StatusVariables.Rejected) ??
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            review.Status = hiddenStatus;
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