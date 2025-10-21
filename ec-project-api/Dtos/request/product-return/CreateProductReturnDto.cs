namespace ec_project_api.Dtos.request.product_return
{
    public class CreateProductReturnDto
    {
        public int OrderItemId { get; set; }             // Sản phẩm đã mua cần hoàn hoặc đổi
        public int ReturnType { get; set; }              // 1 = Đổi hàng, 2 = Hoàn tiền
        public string? ReturnReason { get; set; }        // Lý do đổi/hoàn
        public decimal? ReturnAmount { get; set; }       // Số tiền hoàn (nếu là hoàn tiền)
        public int? ReturnProductVariantId { get; set; } // Sản phẩm thay thế (nếu là đổi hàng)
        public short StatusId { get; set; }                // Trạng thái xử lý
    }
}
