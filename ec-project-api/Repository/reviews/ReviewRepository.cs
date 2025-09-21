using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(DataContext context) : base(context) { }
}
