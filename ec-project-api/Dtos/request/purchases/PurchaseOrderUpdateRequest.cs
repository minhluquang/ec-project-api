namespace ec_project_api.Dtos.request.purchaseorders
{
    public class PurchaseOrderUpdateRequest
    {
        public int Id { get; set; }

        public int? SupplierId { get; set; }

        public DateTime? OrderDate { get; set; }

        public List<PurchaseOrderItemCreateRequest>? Items { get; set; }
    }
}
