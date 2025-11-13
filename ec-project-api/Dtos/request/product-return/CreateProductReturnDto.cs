namespace ec_project_api.Dtos.request.product_return
{
    public class CreateProductReturnDto
    {
        public int OrderItemId { get; set; }             // Sản phẩm đã mua cần hoàn hoặc đổi
        public int ReturnType { get; set; }              // 1 = Đổi hàng, 2 = Hoàn tiền
        public string? ReturnReason { get; set; }        // Lý do đổi/hoàn
        public int quantity { get; set; } = 1;

    }
}
