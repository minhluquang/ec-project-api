using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Reviews {
    public interface IReviewReportService : IBaseService<ReviewReport, int> {
        Task<IEnumerable<ReviewReport>> GetAllByReviewIdAsync(int reviewId, QueryOptions<ReviewReport>? options = null);
        Task<bool> HasUserReportedReviewAsync(int reviewId, int userId);
    }
}