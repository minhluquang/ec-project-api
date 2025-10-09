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
    }
}