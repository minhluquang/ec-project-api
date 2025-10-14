namespace ec_project_api.Constants.Messages {
    public static class OrderMessages {
        // Order related messages
        public const string OrderNotFound = "Không tìm thấy đơn hàng.";
        public const string OrderAlreadyExists = "Đơn hàng đã tồn tại.";
        public const string OrderCreateFailed = "Tạo đơn hàng thất bại.";
        public const string OrderUpdateFailed = "Cập nhật đơn hàng thất bại.";
        public const string OrderDeleteFailed = "Xóa đơn hàng thất bại.";
        public const string InvalidOrderData = "Dữ liệu đơn hàng không hợp lệ.";
        public const string SuccessfullyCreatedOrder = "Tạo đơn hàng thành công.";
        public const string SuccessfullyUpdatedOrder = "Cập nhật đơn hàng thành công.";
        public const string SuccessfullyDeletedOrder = "Xóa đơn hàng thành công.";
        public const string OrderStatusChanged = "Trạng thái đơn hàng đã được thay đổi.";
        public const string OrderCannotBeCancelled = "Không thể hủy đơn hàng này.";
        public const string OrderAlreadyCancelled = "Đơn hàng đã bị hủy.";
        public const string OrderAlreadyCompleted = "Đơn hàng đã hoàn thành.";
        public const string OrderIsEmpty = "Đơn hàng không có sản phẩm.";
        public const string OrderTotalInvalid = "Tổng tiền đơn hàng không hợp lệ.";
        public const string OrdersRetrievedSuccessfully = "Lấy danh sách đơn hàng thành công.";
        public const string OrderRetrievedSuccessfully = "Lấy thông tin đơn hàng thành công.";
        public const string OrderCannotBeDeleted = "Chỉ có thể xóa đơn hàng ở trạng thái nháp (Draft).";

        // Order item related messages
        public const string OrderItemNotFound = "Không tìm thấy sản phẩm trong đơn hàng.";
        public const string OrderItemAlreadyExists = "Sản phẩm đã tồn tại trong đơn hàng.";
        public const string OrderItemCreateFailed = "Thêm sản phẩm vào đơn hàng thất bại.";
        public const string OrderItemUpdateFailed = "Cập nhật sản phẩm trong đơn hàng thất bại.";
        public const string OrderItemDeleteFailed = "Xóa sản phẩm khỏi đơn hàng thất bại.";
        public const string InvalidOrderItemData = "Dữ liệu sản phẩm trong đơn hàng không hợp lệ.";
        public const string SuccessfullyCreatedOrderItem = "Thêm sản phẩm vào đơn hàng thành công.";
        public const string SuccessfullyUpdatedOrderItem = "Cập nhật sản phẩm trong đơn hàng thành công.";
        public const string SuccessfullyDeletedOrderItem = "Xóa sản phẩm khỏi đơn hàng thành công.";
        public const string OrderItemQuantityInvalid = "Số lượng sản phẩm không hợp lệ.";
        public const string OrderItemPriceInvalid = "Giá sản phẩm không hợp lệ.";
        public const string OrderItemOutOfStock = "Sản phẩm không đủ số lượng trong kho.";
        public const string OrderItemSubTotalInvalid = "Tổng tiền sản phẩm không hợp lệ.";
        public const string OrderItemCannotBeModified = "Không thể chỉnh sửa sản phẩm trong đơn hàng này.";
        public const string OrderItemsRetrievedSuccessfully = "Lấy danh sách sản phẩm trong đơn hàng thành công.";
        public const string OrderItemRetrievedSuccessfully = "Lấy thông tin sản phẩm trong đơn hàng thành công.";
        public const string InvalidStatusTransition = "Cập nhật trạng thái đơn hàng không hợp lệ.";
        public const string FinalStatusCannotChange = "Trạng thái cuối cùng không thể thay đổi.";

    }
}