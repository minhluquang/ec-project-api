namespace ec_project_api.Dtos.response.purchaseorders
{
    public class PurchaseOrderItemResponse
    {
        public int PurchaseOrderItemId { get; set; }
        public int ProductVariantId { get; set; }
        public string? Sku { get; set; }
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal ProfitPercentage { get; set; }
        public bool IsPushed { get; set; }
    }
}
