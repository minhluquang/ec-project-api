using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.review_images;
using Microsoft.EntityFrameworkCore;

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
                        );
            options.Filter = r => r.OrderItem != null && r.OrderItem.ProductVariant != null &&
                                  r.OrderItem.ProductVariant.ProductId == productId;
            options.Includes.Add(r => r.ReviewImages);

            return await _reviewRepository.GetAllAsync(options);
        }

        public async Task<bool> CreateReviewAndUploadReviewImagesAsync(Review reivew, List<IFormFile>? images) {
            await base.CreateAsync(reivew);

            if (images != null) {
                foreach (var image in images) {
                    var reviewImage = new ReviewImage
                    {
                        ReviewId = reivew.ReviewId,
                    };
                    await _reviewImageService.UploadSingleReviewImageAsync(reviewImage, image);
                }
            }

            return true;
        }
    }
}