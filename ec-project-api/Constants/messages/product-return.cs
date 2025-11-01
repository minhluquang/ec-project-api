namespace ec_project_api.Constants.messages
{
    public class ProductReturnMessages
    {
        public const string SuccessfullyCreatedReturn = "Tạo yêu cầu hoàn/đổi thành công.";
        public const string SuccessfullyUpdatedReturn = "Cập nhật yêu cầu hoàn/đổi thành công.";
        public const string SuccessfullyDeletedReturn = "Xóa yêu cầu hoàn/đổi thành công.";
        // Product return related messages
        public const string ProductReturnNotFound = "Không tìm thấy yêu cầu đổi trả.";
        public const string ProductReturnAlreadyExists = "Yêu cầu đổi trả đã tồn tại.";
        public const string ProductReturnCreateFailed = "Tạo yêu cầu đổi trả thất bại.";
        public const string ProductReturnUpdateFailed = "Cập nhật yêu cầu đổi trả thất bại.";
        public const string ProductReturnDeleteFailed = "Xóa yêu cầu đổi trả thất bại.";
        public const string InvalidProductReturnData = "Dữ liệu đổi trả không hợp lệ.";
        public const string SuccessfullyCreatedProductReturn = "Tạo yêu cầu đổi trả thành công.";
        public const string SuccessfullyUpdatedProductReturn = "Cập nhật yêu cầu đổi trả thành công.";
        public const string SuccessfullyDeletedProductReturn = "Xóa yêu cầu đổi trả thành công.";
        public const string ProductReturnStatusChanged = "Trạng thái đổi trả đã được thay đổi.";
        public const string ProductReturnCannotBeCancelled = "Không thể hủy yêu cầu đổi trả này.";
        public const string ProductReturnAlreadyCancelled = "Yêu cầu đổi trả đã bị hủy.";
        public const string ProductReturnAlreadyCompleted = "Yêu cầu đổi trả đã hoàn thành.";
        public const string ProductReturnsRetrievedSuccessfully = "Lấy danh sách đổi trả thành công.";
        public const string ProductReturnRetrievedSuccessfully = "Lấy thông tin đổi trả thành công.";
        public const string SuccessfullyApprovedProductReturn = "Phê duyệt yêu cầu đổi trả thành công.";

        // Order item validation messages
        public const string OrderItemNotFound = "Không tìm thấy sản phẩm phù hợp.";
        public const string OrderItemCannotBeReturned = "Sản phẩm này không thể đổi trả.";

        // Exchange (return_type = 1) related messages
        public const string ExchangeRequiresReplacementProduct = "Đổi hàng yêu cầu cung cấp sản phẩm thay thế.";
        public const string ReplacementProductNotFound = "Không tìm thấy sản phẩm thay thế.";
        public const string ReplacementProductOutOfStock = "Sản phẩm thay thế không đủ hàng trong kho.";

        // Refund (return_type = 2) related messages
        public const string RefundRequiresAmount = "Hoàn tiền yêu cầu cung cấp số tiền hoàn.";
        public const string InvalidRefundAmount = "Số tiền hoàn không hợp lệ.";
        public const string RefundAmountExceedsOrderAmount = "Số tiền hoàn vượt quá giá trị đơn hàng.";

        // Stock management messages
        public const string StockUpdateFailed = "Cập nhật tồn kho thất bại.";
        public const string InsufficientStock = "Sản phẩm không đủ hàng trong kho.";

        // Return reason messages
        public const string ReturnReasonRequired = "Vui lòng cung cấp lý do đổi trả.";
        public const string InvalidReturnReason = "Lý do đổi trả không hợp lệ.";

        // Return type messages
        public const string InvalidReturnType = "Loại đổi trả không hợp lệ.";
        public const string ReturnTypeRequired = "Vui lòng chọn loại đổi trả.";

        // Status messages
        public const string InvalidStatusTransition = "Cập nhật trạng thái đổi trả không hợp lệ.";
        public const string StatusRequired = "Vui lòng cung cấp trạng thái đổi trả.";
        public const string ProductReturnCannotBeDeleted = "Chỉ có thể xóa yêu cầu đổi trả ở trạng thái nháp.";
    }
}
