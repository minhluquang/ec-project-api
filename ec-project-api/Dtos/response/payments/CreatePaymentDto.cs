namespace ec_project_api.Dtos.response.payments
{
    public class CreatePaymentDto
    {
        public int? DestinationId { get; set; }
        public int StatusId { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public int? OrderId { get; set; } // gắn Payment vào Order
    }
}
