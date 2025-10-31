using AutoMapper;
using ec_project_api.Dtos.response.inventory;
using ec_project_api.Interfaces.Products;
using ec_project_api.Interfaces.PurchaseOrders;
using ec_project_api.Interfaces.inventory;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Helpers;

namespace ec_project_api.Services.inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductVariantRepository _variantRepo;
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IPurchaseOrderItemRepository _poiRepo;
        private readonly IMapper _mapper;

        public InventoryService(
            IProductVariantRepository variantRepo,
            IPurchaseOrderRepository poRepo,
            IPurchaseOrderItemRepository poiRepo,
            IMapper mapper)
        {
            _variantRepo = variantRepo;
            _poRepo = poRepo;
            _poiRepo = poiRepo;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<InventoryItemDto> Items, int Total)> GetListAsync()
        {
            var options = new QueryOptions<ProductVariant>
            {
                AsNoTracking = true
            };
            options.Includes.Add(v => v.Product!);
            options.Includes.Add(v => v.Size!);
            options.IncludeThen.Add(q => q.Include(v => v.Product)!.ThenInclude(p => p.Category));
            options.IncludeThen.Add(q => q.Include(v => v.Product)!.ThenInclude(p => p.Color));

            var variants = await _variantRepo.GetAllAsync(options);
            var items = _mapper.Map<IEnumerable<InventoryItemDto>>(variants);

            foreach (var item in items)
            {
                item.Status = item.StockQuantity <= 0
                    ? "OutOfStock"
                    : (item.StockQuantity <= 5 ? "LowStock" : "InStock");
            }

            return (items, items.Count());
        }

        public async Task<InventoryItemDto> GetByVariantIdAsync(int productVariantId)
        {
            var options = new QueryOptions<ProductVariant>
            {
                AsNoTracking = true
            };
            options.Includes.Add(v => v.Product!);
            options.Includes.Add(v => v.Size!);
            options.IncludeThen.Add(q => q.Include(v => v.Product)!.ThenInclude(p => p.Category));
            options.IncludeThen.Add(q => q.Include(v => v.Product)!.ThenInclude(p => p.Color));

            var variant = await _variantRepo.GetByIdAsync(productVariantId, options)
                          ?? throw new KeyNotFoundException("Product variant not found");

            var dto = _mapper.Map<InventoryItemDto>(variant);
            dto.Status = dto.StockQuantity <= 0 ? "OutOfStock" : (dto.StockQuantity <= 5 ? "LowStock" : "InStock");
            return dto;
        }
    }


}
