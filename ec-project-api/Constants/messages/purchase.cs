namespace ec_project_api.Constants.Messages
{
    public static class PurchaseOrderMessages
    {
        public const string PurchaseOrderNotFound = "Đơn nhập hàng không tồn tại.";
        public const string PurchaseOrderRetrievedSuccessfully = "Lấy danh sách đơn nhập hàng thành công.";
        public const string PurchaseOrderCreatedSuccessfully = "Tạo đơn nhập hàng thành công.";
        public const string PurchaseOrderUpdatedSuccessfully = "Cập nhật đơn nhập hàng thành công.";
        public const string PurchaseOrderDeletedSuccessfully = "Xóa đơn nhập hàng thành công.";
        public const string PurchaseOrderStatusUpdatedSuccessfully = "Cập nhật trạng thái đơn nhập hàng thành công.";

        public const string PurchaseOrderCreateFailed = "Không thể tạo đơn nhập hàng.";
        public const string PurchaseOrderUpdateFailed = "Không thể cập nhật đơn nhập hàng.";
        public const string PurchaseOrderDeleteFailed = "Không thể xóa đơn nhập hàng.";
        public const string PurchaseOrderStatusUpdateFailed = "Không thể cập nhật trạng thái đơn nhập hàng.";

        public const string SupplierNotFound = "Nhà cung cấp không tồn tại.";


        public const string ProductVariantNotFound = "Biến thể sản phẩm không tồn tại.";
        public const string ProductVariantNotFoundWithId = "Biến thể sản phẩm (ID: {0}) không tồn tại.";


        public const string ItemAddedSuccessfully = "Thêm item vào đơn nhập hàng thành công.";
        public const string ItemUpdatedSuccessfully = "Cập nhật item trong đơn nhập hàng thành công.";
        public const string ItemDeletedSuccessfully = "Xóa item trong đơn nhập hàng thành công.";

        public const string ItemAddFailed = "Không thể thêm item vào đơn nhập hàng.";
        public const string ItemUpdateFailed = "Không thể cập nhật item trong đơn nhập hàng.";
        public const string ItemDeleteFailed = "Không thể xóa item trong đơn nhập hàng.";
    }
}
