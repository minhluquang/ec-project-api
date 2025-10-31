using ec_project_api.Dtos.Statuses;

namespace ec_project_api.Dtos.response.payments
{
    public class PaymentMethodDto
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = null!;
        public string MethodType { get; set; } = null!;
        public StatusDto? Status { get; set; }
    }
}
