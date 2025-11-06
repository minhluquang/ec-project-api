using ec_project_api.Controllers;
using ec_project_api.Dtos.request.orders;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.cart_items
{
    public interface ICartItemService : IBaseService<CartItem, int>
    {
        Task<bool> UpdateCartItemAsync(int userId, int productVariantId, short quantity, decimal price);
        Task<bool> CreateOrUpdateCartItemAsync(CartUpdateRequest request);
        Task<bool> RemoveCartItemAsync(int userId, int variantId);
        Task<bool> ClearCartAsync(int userId);
    }
}
