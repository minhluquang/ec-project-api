using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.ReviewReports {
    public class ReviewReportService : BaseService<ReviewReport, int>, IReviewReportService {
        private readonly IReviewReportRepository _reviewReportRepository;

        public ReviewReportService(IReviewReportRepository reviewReportRepository) : base(reviewReportRepository) {
            _reviewReportRepository = reviewReportRepository;
        }

        public async Task<IEnumerable<ReviewReport>> GetAllByReviewIdAsync(int reviewId, QueryOptions<ReviewReport>? options = null) {
            options ??= new QueryOptions<ReviewReport>();

            options.Includes.Add(rr => rr.User!);
            options.Includes.Add(rr => rr.Status!);
            options.Includes.Add(rr => rr.Review!);
            options.Filter = rr => rr.ReviewId == reviewId;

            return await _reviewReportRepository.GetAllAsync(options);
        }

        public async Task<bool> HasUserReportedReviewAsync(int reviewId, int userId) {
            var existingReport = await _reviewReportRepository.FirstOrDefaultAsync(
                rr => rr.ReviewId == reviewId && rr.UserId == userId
            );
            return existingReport != null;
        }

        public override async Task<ReviewReport?> GetByIdAsync(int id, QueryOptions<ReviewReport>? options = null) {
            options ??= new QueryOptions<ReviewReport>();

            options.Includes.Add(rr => rr.User!);
            options.Includes.Add(rr => rr.Status!);
            options.Includes.Add(rr => rr.Review!);

            return await base.GetByIdAsync(id, options);
        }
    }
}