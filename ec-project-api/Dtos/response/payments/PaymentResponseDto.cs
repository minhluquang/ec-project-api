namespace ec_project_api.Dtos.response.payments
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? StatusName { get; set; }
        public string? DestinationName { get; set; }
        public int? OrderId { get; set; }
    }
}
