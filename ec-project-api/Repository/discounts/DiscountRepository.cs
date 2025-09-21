using ec_project_api.Interfaces.Discounts;
using ec_project_api.Models;

public class DiscountRepository : Repository<Discount>, IDiscountRepository
{
    public DiscountRepository(DataContext context) : base(context) { }
}
