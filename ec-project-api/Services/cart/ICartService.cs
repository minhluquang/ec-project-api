using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.cart
{
    public interface ICartService : IBaseService<Cart, int>
    {
        Task<Cart?> GetCartWithItemsAsync(int userId, QueryOptions<Cart>? options = null);
    }
}
