using ec_project_api.Dtos.request.orders;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Services.Bases;
using ec_project_api.Services.cart_items;

public class CartItemService : BaseService<CartItem, int>, ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly ICartRepository _cartRepository;

    public CartItemService(ICartItemRepository cartItemRepository, ICartRepository cartRepository)
        : base(cartItemRepository)
    {
        _cartItemRepository = cartItemRepository;
        _cartRepository = cartRepository;
    }

    public async Task<bool> CreateOrUpdateCartItemAsync(CartUpdateRequest request)
    {
        // 🔍 Tìm giỏ hàng của user
        var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == request.UserId);
        if (cart == null)
        {
            cart = new Cart { UserId = request.UserId };
            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }

        // 🔍 Tìm item trong giỏ
        var existingItem = await _cartItemRepository.FirstOrDefaultAsync(
            i => i.CartId == cart.CartId && i.ProductVariantId == request.VariantId);

        if (existingItem != null)
        {
            existingItem.Quantity = request.Quantity;
            existingItem.Price = request.Price;
            existingItem.Slug = request.Slug;
            await _cartItemRepository.UpdateAsync(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.CartId,
                ProductVariantId = request.VariantId,
                Quantity = request.Quantity,
                Price = request.Price,
                Slug = request.Slug
            };
            await _cartItemRepository.AddAsync(newItem);
        }

        return await _cartItemRepository.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCartItemAsync(int userId, int productVariantId, short quantity, decimal price)
    {
        // 1. Lấy giỏ hàng của user
        var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            // Nếu chưa có giỏ hàng thì tạo mới
            cart = new Cart { UserId = userId };
            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();
        }

        // 2. Tìm CartItem hiện có
        var existingItem = await _cartItemRepository.FirstOrDefaultAsync(ci =>
            ci.CartId == cart.CartId && ci.ProductVariantId == productVariantId);

        if (existingItem != null)
        {
            if (quantity <= 0)
            {
                // Xoá item nếu quantity = 0
                await _cartItemRepository.DeleteAsync(existingItem);
            }
            else
            {
                // Cập nhật số lượng & giá
                existingItem.Quantity = quantity;
                existingItem.Price = price;
                await _cartItemRepository.UpdateAsync(existingItem);
            }
        }
        else if (quantity > 0)
        {
            // Thêm mới nếu chưa có
            var newItem = new CartItem
            {
                CartId = cart.CartId,
                ProductVariantId = productVariantId,
                Quantity = quantity,
                Price = price,
                Slug = string.Empty // Slug sẽ cần được truyền vào nếu cần
            };
            await _cartItemRepository.AddAsync(newItem);
        }

        return await _cartItemRepository.SaveChangesAsync() > 0;
    }
    public async Task<bool> RemoveCartItemAsync(int userId, int variantId)
    {
        var cart = await _cartRepository.FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return false;

        var item = await _cartItemRepository.FirstOrDefaultAsync(
            i => i.CartId == cart.CartId && i.ProductVariantId == variantId);

        if (item == null) return false;

        await _cartItemRepository.DeleteAsync(item);
        return await _cartItemRepository.SaveChangesAsync() > 0;
    }
}
