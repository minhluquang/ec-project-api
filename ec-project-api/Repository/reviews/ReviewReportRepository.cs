using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;
using ec_project_api.Repository.Base;

namespace ec_project_api.Repository.ReviewReports {
    public class ReviewReportRepository : Repository<ReviewReport, int>, IReviewReportRepository {
        public ReviewReportRepository(DataContext context) : base(context) {
        }
    }
}