using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.Reviews {
    public class ReviewService : BaseService<Review, int>, IReviewService {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository, DataContext dbContext) : base(reviewRepository) {
            _reviewRepository = reviewRepository;
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
    }
}