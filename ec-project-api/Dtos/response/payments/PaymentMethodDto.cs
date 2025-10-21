namespace ec_project_api.Dtos.response.payments
{
    public class PaymentMethodDto
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = null!;
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
    }
}
