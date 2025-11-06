using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.inventory
{
    public static class BatchInventoryExtensions
    {
        public static async Task<List<PurchaseOrderItem>> GetAllBatchesAsync(
            this DataContext context, 
            int productVariantId)
        {
            return await context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId)
                .OrderBy(poi => poi.CreatedAt)
                .ToListAsync();
        }

        public static async Task<List<PurchaseOrderItem>> GetActiveBatchesAsync(
            this DataContext context, 
            int productVariantId)
        {
            return await context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && poi.IsPushed)
                .OrderBy(poi => poi.CreatedAt)
                .ToListAsync();
        }

        public static async Task<List<PurchaseOrderItem>> GetInactiveBatchesAsync(
            this DataContext context, 
            int productVariantId)
        {
            return await context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && !poi.IsPushed)
                .OrderBy(poi => poi.CreatedAt)
                .ToListAsync();
        }

        public static async Task<int> GetTotalStockAsync(
            this DataContext context, 
            int productVariantId)
        {
            var variant = await context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);
            
            var availableStock = variant?.StockQuantity ?? 0;
            
            var inactiveStock = await context.PurchaseOrderItems
                .Where(poi => poi.ProductVariantId == productVariantId && !poi.IsPushed)
                .SumAsync(poi => (int)poi.Quantity);

            return availableStock + inactiveStock;
        }
        public static async Task<bool> CanFulfillOrderAsync(
            this DataContext context, 
            int productVariantId, 
            int quantityNeeded)
        {
            var totalStock = await context.GetTotalStockAsync(productVariantId);
            return totalStock >= quantityNeeded;
        }

        public static async Task<BatchInventorySummary> GetInventorySummaryAsync(
            this DataContext context, 
            int productVariantId)
        {
            var allBatches = await context.GetAllBatchesAsync(productVariantId);
            var activeBatches = allBatches.Where(b => b.IsPushed).ToList();
            var inactiveBatches = allBatches.Where(b => !b.IsPushed).ToList();

            var variant = await context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductVariantId == productVariantId);
            var availableStock = variant?.StockQuantity ?? 0;

            return new BatchInventorySummary
            {
                ProductVariantId = productVariantId,
                TotalBatches = allBatches.Count,
                ActiveBatches = activeBatches.Count,
                InactiveBatches = inactiveBatches.Count,
                AvailableStock = availableStock, 
                InactiveStock = inactiveBatches.Sum(b => (int)b.Quantity), 
                TotalStock = availableStock + inactiveBatches.Sum(b => (int)b.Quantity), 
                AverageUnitPrice = allBatches.Any() 
                    ? allBatches.Average(b => b.UnitPrice) 
                    : 0,
                CurrentSellingPrice = activeBatches.Any()
                    ? activeBatches.First().UnitPrice * (1 + activeBatches.First().ProfitPercentage / 100)
                    : 0
            };
        }
    }

    public class BatchInventorySummary
    {
        public int ProductVariantId { get; set; }
        public int TotalBatches { get; set; }
        public int ActiveBatches { get; set; }
        public int InactiveBatches { get; set; }
        public int AvailableStock { get; set; }
        public int InactiveStock { get; set; }
        public int TotalStock { get; set; }
        public decimal AverageUnitPrice { get; set; }
        public decimal CurrentSellingPrice { get; set; }
    }
}
