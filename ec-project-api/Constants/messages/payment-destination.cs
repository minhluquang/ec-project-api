namespace ec_project_api.Constants.messages
{
    public static class PaymentMessages
    {
        // Thông báo chung
        public const string NotFound = "Không tìm thấy điểm thanh toán.";

        // Thành công
        public const string PaymentDestinationRetrievedSuccessfully = "Lấy danh sách điểm thanh toán thành công.";
        public const string PaymentDestinationCreatedSuccessfully = "Tạo điểm thanh toán mới thành công.";
        public const string PaymentDestinationUpdatedSuccessfully = "Cập nhật thông tin điểm thanh toán thành công.";
        public const string PaymentDestinationDeletedSuccessfully = "Xóa điểm thanh toán thành công.";
        public const string PaymentDestinationStatusUpdatedSuccessfully = "Cập nhật trạng thái điểm thanh toán thành công.";

        // Thất bại
        public const string PaymentDestinationCreatedFailed = "Tạo điểm thanh toán thất bại.";
        public const string PaymentDestinationUpdatedFailed = "Cập nhật điểm thanh toán thất bại.";
        public const string PaymentDestinationDeletedFailed = "Xóa điểm thanh toán thất bại.";
        public const string PaymentDestinationStatusUpdatedFailed = "Cập nhật trạng thái điểm thanh toán thất bại.";

        // Lỗi khác
        public const string InvalidPaymentData = "Dữ liệu thanh toán không hợp lệ.";
        public const string DuplicatePaymentDestination = "Điểm thanh toán đã tồn tại.";
        public const string PaymentProcessingError = "Đã xảy ra lỗi trong quá trình xử lý thanh toán.";
    }
}
