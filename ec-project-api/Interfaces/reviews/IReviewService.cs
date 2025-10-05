using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Reviews {
    public interface IReviewService : IBaseService<Review, int> {
        Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId, QueryOptions<Review>? options = null);
    }
}