namespace ec_project_api.Dtos.response.purchaseorders
{
    public class PurchaseOrderResponse
    {
        public int PurchaseOrderId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime OrderDate { get; set; }
        public short StatusId { get; set; }
        public string? StatusName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PurchaseOrderItemResponse> Items { get; set; } = new();
    }
}
