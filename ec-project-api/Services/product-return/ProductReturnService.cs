using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.product_return;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.productReturn
{
    public class ProductReturnService : BaseService<ProductReturn, int>, IProductReturnService
    {
        private readonly IProductReturnRepository _productReturnRepository;

        public ProductReturnService(IProductReturnRepository productReturnRepository)
            : base(productReturnRepository)
        {
            _productReturnRepository = productReturnRepository;
        }

        public override async Task<IEnumerable<ProductReturn>> GetAllAsync(QueryOptions<ProductReturn>? options = null)
        {
            options ??= new QueryOptions<ProductReturn>();

            // Bao gồm các liên kết cần thiết
            options.Includes.Add(r => r.OrderItem!);
            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem!)
                    .ThenInclude(oi => oi.ProductVariant));
            options.Includes.Add(r => r.ReturnProductVariant!);
            options.Includes.Add(r => r.Status);

            return await _productReturnRepository.GetAllAsync(options);
        }

        public override async Task<ProductReturn?> GetByIdAsync(int id, QueryOptions<ProductReturn>? options = null)
        {
            options ??= new QueryOptions<ProductReturn>();

            options.Includes.Add(r => r.OrderItem!);
            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem!)
                    .ThenInclude(oi => oi.ProductVariant));
            options.Includes.Add(r => r.ReturnProductVariant!);
            options.Includes.Add(r => r.Status);

            return await _productReturnRepository.GetByIdAsync(id, options);
        }

        public async Task<IEnumerable<ProductReturn>> GetByOrderItemIdAsync(int orderItemId, QueryOptions<ProductReturn>? options = null)
        {
            options ??= new QueryOptions<ProductReturn>();
            options.Includes.Add(r => r.OrderItem!);
            options.IncludeThen.Add(q => q
                .Include(r => r.OrderItem!)
                    .ThenInclude(oi => oi.ProductVariant));
            options.Includes.Add(r => r.ReturnProductVariant!);
            options.Includes.Add(r => r.Status);
            
            options.Filter = r => r.OrderItemId == orderItemId;
            
            return await _productReturnRepository.GetAllAsync(options);
        }
    }
}
