using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.review_images;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Dtos.response.reviews;
using ec_project_api.Interfaces.Orders;

namespace ec_project_api.Services.reviews {
    public class ReviewService : BaseService<Review, int>, IReviewService {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewImageService _reviewImageService;

        public ReviewService(IReviewRepository reviewRepository, IReviewImageService reviewImageService, DataContext dbContext) : base(reviewRepository) {
            _reviewRepository = reviewRepository;
            _reviewImageService = reviewImageService;
        }

        public async Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId, QueryOptions<Review>? options = null) {
            options ??= new QueryOptions<Review>();

            options.IncludeThen.Add(q => q
                            .Include(p => p.OrderItem!)
                                .ThenInclude(v => v!.ProductVariant!)
                                    .ThenInclude(pv => pv.Size));

            options.IncludeThen.Add(q => q
                            .Include(r => r.OrderItem)
                                .ThenInclude(oi => oi!.ProductVariant)
                                    .ThenInclude(pv => pv!.Product)
                                        .ThenInclude(p => p!.Color));

            options.Filter = r => r.OrderItem != null && r.OrderItem.ProductVariant != null &&
                                  r.OrderItem.ProductVariant.ProductId == productId;
            options.Includes.Add(r => r.ReviewImages);

            return await _reviewRepository.GetAllAsync(options);
        }

        public async Task<bool> CreateReviewAndUploadReviewImagesAsync(Review review, List<IFormFile>? images) {
            await base.CreateAsync(review);

            if (images != null) {
                foreach (var image in images) {
                    var reviewImage = new ReviewImage
                    {
                        ReviewId = review.ReviewId,
                    };
                    await _reviewImageService.UploadSingleReviewImageAsync(reviewImage, image);
                }
            }

            return true;
        }

        public override async Task<Review?> GetByIdAsync(int reviewId, QueryOptions<Review>? options = null) {
            options ??= new QueryOptions<Review>();

            options.IncludeThen.Add(q => q
                            .Include(p => p.OrderItem)
                                .ThenInclude(v => v!.ProductVariant!)
                                    .ThenInclude(pv => pv!.Size!));

            options.IncludeThen.Add(q => q
                                    .Include(p => p.OrderItem!)
                                        .ThenInclude(v => v!.ProductVariant!)
                                            .ThenInclude(pv => pv!.Product!)
                                                .ThenInclude(p => p!.Color!));

            options.IncludeThen.Add(q => q
                            .Include(p => p.OrderItem)
                                .ThenInclude(oi => oi!.Order));

            options.Filter = r => r.ReviewId == reviewId;
            options.Includes.Add(r => r.ReviewImages);

            return await _reviewRepository.GetByIdAsync(reviewId, options);
        }
        
        public async Task<ReviewSummaryDto> GetSummaryByProductIdAsync(int productId)
        {
            var options = new QueryOptions<Review>
            {
                Filter = r =>
                    r.OrderItem != null &&
                    r.OrderItem.ProductVariant != null &&
                    r.OrderItem.ProductVariant.ProductId == productId &&
                    r.OrderItem.Order != null &&
                    r.OrderItem.Order.Status != null &&
                    r.OrderItem.Order.Status.Name == StatusVariables.Delivered
            };

            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem!)
                .ThenInclude(oi => oi.Order!));
            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.ProductVariant));

            var reviews = await _reviewRepository.GetAllAsync(options);
            if (!reviews.Any())
                return new ReviewSummaryDto { AverageRating = 0, ReviewCount = 0 };
            
            var reviewCount = reviews.Count();
            var averageRating = reviewCount > 0 ? reviews.Average(r => r.Rating) : 0.0;

            return new ReviewSummaryDto
            {
                AverageRating = averageRating,
                ReviewCount = reviewCount
            };
        }
    }
}