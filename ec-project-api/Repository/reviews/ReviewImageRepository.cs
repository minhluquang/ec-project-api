using ec_project_api.Interfaces.Reviews;
using ec_project_api.Models;

public class ReviewImageRepository : Repository<ReviewImage, int>, IReviewImageRepository
{
    public ReviewImageRepository(DataContext context) : base(context) { }
}
