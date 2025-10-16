namespace ec_project_api.Dtos.request.payments
{
    public class PaymentDestinationFilter
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public int? StatusId { get; set; }
        public string? Identifier { get; set; }
        public string? OrderBy { get; set; }
    }
}
