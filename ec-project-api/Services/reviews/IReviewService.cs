using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Dtos.response.reviews;

namespace ec_project_api.Services.reviews {
    public interface IReviewService : IBaseService<Review, int> {
        Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId, QueryOptions<Review>? options = null);
        Task<bool> CreateReviewAndUploadReviewImagesAsync(Review reivew, List<IFormFile>? images);
        Task<ReviewSummaryDto> GetSummaryByProductIdAsync(int productId);
    }
}
