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

        // Status restrictions
        public const string CannotEditInCurrentStatus = "Không thể chỉnh sửa đơn hàng ở trạng thái hiện tại.";
        public const string CannotDeleteInCurrentStatus = "Không thể xóa đơn hàng ở trạng thái hiện tại. Chỉ có thể xóa đơn hàng ở trạng thái Draft.";
        public const string CanOnlyCancelNotDelete = "Không thể xóa đơn hàng ở trạng thái này. Vui lòng hủy đơn hàng thay vì xóa.";
        public const string CannotModifyAfterApproved = "Không thể chỉnh sửa đơn hàng sau khi đã được duyệt.";
        public const string CannotModifyProductsInPending = "Không thể chỉnh sửa danh sách sản phẩm khi đơn hàng ở trạng thái Pending.";
        public const string CannotModifyCancelled = "Không thể thực hiện thao tác trên đơn hàng đã bị hủy.";
        public const string InvalidStatusTransition = "Chuyển trạng thái không hợp lệ.";
    }
}
