using ec_project_api.Dtos.response.inventory;

namespace ec_project_api.Interfaces.inventory
{
    public interface IInventoryService 
    {
        Task<(IEnumerable<InventoryItemDto> Items, int Total)> GetListAsync();
        Task<InventoryItemDto> GetByVariantIdAsync(int productVariantId);
    }
}
