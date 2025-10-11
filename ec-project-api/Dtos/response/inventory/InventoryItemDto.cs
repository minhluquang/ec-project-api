using System;

namespace ec_project_api.Dtos.response.inventory
{
    public class InventoryItemDto
    {
        public int ProductVariantId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int StockQuantity { get; set; }
        public string Status { get; set; } = string.Empty; // InStock/OutOfStock/LowStock
        public DateTime UpdatedAt { get; set; }
    }
}
