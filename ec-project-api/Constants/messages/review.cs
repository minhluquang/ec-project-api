namespace ec_project_api.Constants.messages {
    public static class ReviewMessages {
        // Review related messages
        public const string ReviewNotFound = "Không tìm thấy đánh giá.";
        public const string ReviewAlreadyExists = "Đánh giá đã tồn tại.";
        public const string ReviewCreateFailed = "Tạo đánh giá thất bại.";
        public const string ReviewUpdateFailed = "Cập nhật đánh giá thất bại.";
        public const string ReviewUpdateAlreadyEdited = "Không thể cập nhật đánh giá lần thứ 2.";
        public const string ReviewDeleteFailed = "Xóa đánh giá thất bại.";
        public const string InvalidReviewData = "Dữ liệu đánh giá không hợp lệ.";
        public const string SuccessfullyCreatedReview = "Tạo đánh giá thành công.";
        public const string SuccessfullyUpdatedReview = "Cập nhật đánh giá thành công.";
        public const string SuccessfullyDeletedReview = "Xóa đánh giá thành công.";
        public const string ReviewNotBelongToUser = "Đánh giá không thuộc về người dùng này.";
        public const string ReviewAlreadyExistsForOrderItem = "Đã có đánh giá cho sản phẩm này trong đơn hàng.";
        public const string CannotReviewUndeliveredOrder = "Không thể đánh giá sản phẩm chưa được giao.";
        public const string InvalidRating = "Đánh giá phải từ 1 đến 5 sao.";
        public const string ReviewsRetrievedSuccessfully = "Lấy danh sách đánh giá thành công.";

        // Review image related messages
        public const string ReviewImageNotFound = "Không tìm thấy hình ảnh đánh giá.";
        public const string ReviewImageUploadFailed = "Tải lên hình ảnh đánh giá thất bại.";
        public const string ReviewImageUploadSuccessfully = "Tải lên hình ảnh đánh giá thành công.";
        public const string ReviewImageUpdateFailed = "Cập nhật hình ảnh đánh giá thất bại.";
        public const string ReviewImageDeleteFailed = "Xóa hình ảnh đánh giá thất bại.";
        public const string SuccessfullyDeletedReviewImage = "Xóa hình ảnh đánh giá thành công.";
        public const string ReviewImageDeleteCloudinaryFailed = "Xóa hình ảnh đánh giá trên Cloudinary thất bại.";
        public const string InvalidReviewImageData = "Dữ liệu hình ảnh đánh giá không hợp lệ.";
        public const string TooManyReviewImages = "Quá nhiều hình ảnh đánh giá. Tối đa 5 hình ảnh.";

        // Review report ralated messages
        public const string ReviewReportNotFound = "Không tìm thấy báo cáo đánh giá.";
        public const string ReviewReportAlreadyExists = "Bạn đã báo cáo đánh giá này rồi.";
        public const string ReviewReportCreateFailed = "Tạo báo cáo đánh giá thất bại.";
        public const string ReviewReportUpdateFailed = "Cập nhật báo cáo đánh giá thất bại.";
        public const string ReviewReportDeleteFailed = "Xóa báo cáo đánh giá thất bại.";
        public const string InvalidReviewReportData = "Dữ liệu báo cáo đánh giá không hợp lệ.";
        public const string SuccessfullyCreatedReviewReport = "Tạo báo cáo đánh giá thành công.";
        public const string SuccessfullyUpdatedReviewReport = "Cập nhật báo cáo đánh giá thành công.";
        public const string SuccessfullyDeletedReviewReport = "Xóa báo cáo đánh giá thành công.";
        public const string CannotReportOwnReview = "Không thể báo cáo đánh giá của chính mình.";
        public const string ReasonRequired = "Vui lòng nhập lý do báo cáo.";
        public const string DescriptionTooLong = "Mô tả không được vượt quá 1000 ký tự.";
    }
}