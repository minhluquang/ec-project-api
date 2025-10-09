using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.review_images {
    public interface IReviewImageService : IBaseService<ReviewImage, int> {
        Task<bool> UploadSingleReviewImageAsync(ReviewImage reviewImage, IFormFile fileImage);
        Task<bool> DeleteSingleReviewImageAsync(ReviewImage reviewImage);
    }
}
