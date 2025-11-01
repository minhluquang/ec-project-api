namespace ec_project_api.Dtos.response.purchaseorders
{
    public class PurchaseOrderStatisticsResponse
    {
        public int TotalOrders { get; set; }
        public int DraftOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int OrderedOrders { get; set; }
        public int ReceivedOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalProducts { get; set; }
    }
}
