namespace ec_project_api.Constants.messages
{
    public static class PaymentDestinationMessages
    {
        // General messages
        public const string PaymentDestinationNotFound = "Không tìm thấy điểm đến thanh toán.";
        public const string PaymentDestinationGetAllFailed = "Lấy danh sách điểm thanh toán thất bại";
        public const string PaymentDestinationAlreadyExists = "Điểm đến thanh toán đã tồn tại.";
        public const string PaymentDestinationIdentifierAlreadyExists = "Mã định danh điểm đến thanh toán đã tồn tại.";
        public const string PaymentDestinationCreateFailed = "Tạo điểm đến thanh toán thất bại.";
        public const string PaymentDestinationUpdateFailed = "Cập nhật điểm đến thanh toán thất bại.";
        public const string PaymentDestinationDeleteFailed = "Xóa điểm đến thanh toán thất bại.";
        public const string InvalidPaymentDestinationData = "Dữ liệu điểm đến thanh toán không hợp lệ.";

        // Success messages
        public const string SuccessfullyCreatedPaymentDestination = "Tạo điểm đến thanh toán thành công.";
        public const string SuccessfullyUpdatedPaymentDestination = "Cập nhật điểm đến thanh toán thành công.";
        public const string SuccessfullyDeletedPaymentDestination = "Xóa điểm đến thanh toán thành công.";

        // Status related
        public const string PaymentDestinationStatusUpdateFailed = "Cập nhật trạng thái điểm đến thanh toán thất bại.";
        public const string SuccessfullyUpdatedPaymentDestinationStatus = "Cập nhật trạng thái điểm đến thanh toán thành công.";

        // Retrieval messages
        public const string PaymentDestinationRetrievedSuccessfully = "Lấy thông tin điểm đến thanh toán thành công.";
        public const string PaymentDestinationsRetrievedSuccessfully = "Lấy danh sách điểm đến thanh toán thành công.";

        // Validation or relation
        public const string PaymentDestinationInUse = "Không thể xóa điểm đến thanh toán vì đang được sử dụng.";
        public const string PaymentDestinationBankNameAlreadyExists = "Ngân hàng của điểm đến thanh toán đã tồn tại.";
        public const string PaymentDestinationImageUpdateFailed = "Cập nhật hình ảnh điểm đến thanh toán thất bại.";

    }
}
