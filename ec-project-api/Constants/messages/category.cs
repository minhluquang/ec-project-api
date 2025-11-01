namespace ec_project_api.Constants.messages
{
    public static class CategoryMessages
    {
        public const string CategoryNotFound = "Không tìm thấy thể loại.";
        public const string CategoryAlreadyExists = "Thể loại đã tồn tại.";
        public const string CategorySlugAlreadyExists = "Slug của thể loại đã tồn tại.";
        public const string CategoryCreateFailed = "Tạo thể loại thất bại.";
        public const string CategoryUpdateFailed = "Cập nhật thể loại thất bại.";
        public const string CategoryDeleteFailed = "Xóa thể loại thất bại.";
        public const string InvalidCategoryData = "Dữ liệu thể loại không hợp lệ.";
        public const string InvalidCategoryId = "ID thể loại không hợp lệ.";
        public const string SuccessfullyCreatedCategory = "Tạo thể loại thành công.";
        public const string SuccessfullyUpdatedCategory = "Cập nhật thể loại thành công.";
        public const string SuccessfullyDeletedCategory = "Xóa thể loại thành công.";
        public const string CategoryInUse = "Thể loại đang được sử dụng, không thể xóa.";
        public const string CategoryDeleteFailedNotInactive = "Chỉ có thể xóa thể loại ở trạng thái không hoạt động.";
        public const string CategoryRetrievedSuccessfully = "Lấy danh sách thể loại.";
        public const string CategoryOriginDeleteFailed = "Không thể xóa thể loại cấp gốc (ID = 1 hoặc 2).";
        public const string CategoryOriginUpdateFailed = "Không thể sửa thể loại cấp gốc (ID = 1 hoặc 2).";
        public const string CategoryHasChild = "Không thể xóa thể loại là cha của thể loại đang được sử dụng.";
    }
}