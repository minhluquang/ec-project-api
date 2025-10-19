namespace ec_project_api.Constants.messages
{
    public static class PaymentMethodMessages
    {
        // General messages
        public const string PaymentMethodNotFound = "Không tìm thấy phương thức thanh toán.";
        public const string PaymentMethodAlreadyExists = "Phương thức thanh toán đã tồn tại.";
        public const string PaymentMethodNameAlreadyExists = "Tên phương thức thanh toán đã tồn tại.";
        public const string PaymentMethodCreateFailed = "Tạo phương thức thanh toán thất bại.";
        public const string PaymentMethodUpdateFailed = "Cập nhật phương thức thanh toán thất bại.";
        public const string PaymentMethodDeleteFailed = "Xóa phương thức thanh toán thất bại.";
        public const string InvalidPaymentMethodData = "Dữ liệu phương thức thanh toán không hợp lệ.";

        // Success messages
        public const string SuccessfullyCreatedPaymentMethod = "Tạo phương thức thanh toán thành công.";
        public const string SuccessfullyUpdatedPaymentMethod = "Cập nhật phương thức thanh toán thành công.";
        public const string SuccessfullyDeletedPaymentMethod = "Xóa phương thức thanh toán thành công.";

        // Status related
        public const string PaymentMethodStatusUpdateFailed = "Cập nhật trạng thái phương thức thanh toán thất bại.";
        public const string SuccessfullyUpdatedPaymentMethodStatus = "Cập nhật trạng thái phương thức thanh toán thành công.";

        // Retrieval messages
        public const string PaymentMethodRetrievedSuccessfully = "Lấy thông tin phương thức thanh toán thành công.";
        public const string PaymentMethodsRetrievedSuccessfully = "Lấy danh sách phương thức thanh toán thành công.";

        // Validation or relation
        public const string PaymentMethodInUse = "Không thể xóa phương thức thanh toán vì đang được sử dụng.";
        public const string PaymentMethodCodeAlreadyExists = "Mã phương thức thanh toán đã tồn tại.";
    }
}
