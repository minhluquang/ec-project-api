namespace ec_project_api.Dtos.response.product_return
{
    public class ProductReturnDto
    {
        public int ReturnId { get; set; }
        public int OrderItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ReturnProductName { get; set; }
        public int ReturnType { get; set; }
        public string ReturnTypeName => ReturnType == 1 ? "Đổi hàng" : "Hoàn tiền";
        public string? ReturnReason { get; set; }
        public decimal? ReturnAmount { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
