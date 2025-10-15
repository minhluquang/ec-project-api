namespace ec_project_api.Constants.Messages
{
    public static class PurchaseOrderMessages
    {
        // Đơn nhập hàng
        public const string PurchaseOrderNotFound = "Đơn nhập hàng không tồn tại.";
        public const string PurchaseOrderCreateFailed = "Không thể tạo đơn nhập hàng.";
        public const string PurchaseOrderDeleteFailed = "Không thể xóa đơn nhập hàng.";
        public const string PurchaseOrderUpdateFailed = "Không thể cập nhật trạng thái đơn nhập hàng.";

        // Nhà cung cấp
        public const string SupplierNotFound = "Nhà cung cấp không tồn tại.";

        // Biến thể sản phẩm
        public const string ProductVariantNotFound = "Biến thể sản phẩm không tồn tại.";
        public const string ProductVariantNotFoundWithId = "Biến thể sản phẩm (ID: {0}) không tồn tại.";

        // Item trong đơn nhập
        public const string ItemDeleteFailed = "Không thể xóa item trong đơn nhập hàng.";
    }
}
