using ec_project_api.Models;

namespace ec_project_api.Services.inventory
{
    public interface IBatchInventoryService
    {
        Task<List<BatchDeductionResult>> DeductFromBatchesAsync(int productVariantId, int quantityToDeduct);
        Task ReturnToBatchesAsync(int productVariantId, int quantityToReturn);
        Task<int> GetAvailableStockAsync(int productVariantId);
        Task<bool> ActivateNextBatchAsync(int productVariantId, int quantityNeeded);
        Task<decimal> GetCurrentSellingPriceAsync(int productVariantId);
        Task<decimal> CalculateAveragePriceAsync(int productVariantId, int quantity);
    }
    public class BatchDeductionResult
    {
        public int PurchaseOrderItemId { get; set; }
        public int QuantityDeducted { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ProfitPercentage { get; set; }
    }
}
