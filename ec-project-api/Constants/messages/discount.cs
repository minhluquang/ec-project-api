namespace ec_project_api.Constants.messages
{
    public static class DiscountMessages
    {
        public const string DiscountNotFound = "Không tìm thấy mã giảm giá.";
        public const string DiscountAlreadyExists = "Mã giảm giá đã tồn tại.";
        public const string DiscountCodeAlreadyExists = "Mã code giảm giá đã tồn tại.";
        public const string DiscountCreateFailed = "Tạo mã giảm giá thất bại.";
        public const string DiscountUpdateFailed = "Cập nhật mã giảm giá thất bại.";
        public const string DiscountDeleteFailed = "Xóa mã giảm giá thất bại.";
        public const string DiscountDeleteFailedNotInActive = "Chỉ có thể xóa mã giảm giá ở trạng thái ngừng áp dụng.";
        public const string InvalidDiscountData = "Dữ liệu mã giảm giá không hợp lệ.";
        public const string InvalidDiscountId = "ID mã giảm giá không hợp lệ.";
        public const string InvalidDiscountType = "Loại giảm giá không hợp lệ.";
        public const string InvalidDiscountValue = "Giá trị giảm giá không hợp lệ.";
        public const string SuccessfullyCreatedDiscount = "Tạo mã giảm giá thành công.";
        public const string SuccessfullyUpdatedDiscount = "Cập nhật mã giảm giá thành công.";
        public const string SuccessfullyDeletedDiscount = "Xóa mã giảm giá thành công.";
        public const string DiscountInUse = "Mã giảm giá đang được sử dụng, không thể xóa.";
        public const string DiscountUsed = "Mã giảm giá đã được sử dụng, không thể xóa.";
        public const string StatusNotFound = "Không tìm thấy trạng thái cho mã giảm giá.";
        public const string DiscountRetrievedSuccessfully = "Lấy danh sách mã giảm giá thành công.";
    }
}