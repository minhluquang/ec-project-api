using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.response.inventory;
using ec_project_api.Interfaces.inventory;

namespace ec_project_api.Facades.inventory
{
    public class InventoryFacade
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;

        public InventoryFacade(IInventoryService inventoryService, IMapper mapper)
        {
            _inventoryService = inventoryService;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<InventoryItemDto> Items, int Total)> GetListAsync()
        {
            return await _inventoryService.GetListAsync();
        }

        public async Task<InventoryItemDto> GetByVariantIdAsync(int productVariantId)
        {
            if (productVariantId <= 0)
                throw new ArgumentException(InventoryMessages.InvalidProductVariantId);

            var result = await _inventoryService.GetByVariantIdAsync(productVariantId);
            if (result == null)
                throw new KeyNotFoundException(InventoryMessages.ProductVariantNotFound);

            return result;
        }
        
        
    }
}
