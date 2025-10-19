namespace ec_project_api.DTOs.Payments
{
    public class PaymentDestinationDto
    {
        public int DestinationId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        //public string ImageUrl { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public short StatusId { get; set; }
        public string? StatusName { get; set; }    // lấy từ navigation property Status.DisplayName
        public string? PaymentMethodName { get; set; } // lấy từ PaymentMethod.MethodName
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
