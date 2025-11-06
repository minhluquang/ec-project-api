namespace ec_project_api.Constants.messages
{
    public static class ProductGroupMessages
    {
        public const string ProductGroupNotFound = "Không tìm thấy nhóm sản phẩm.";
        public const string ProductGroupNameAlreadyExists = "Tên nhóm sản phẩm đã tồn tại.";
        public const string SuccessfullyCreatedProductGroup = "Tạo nhóm sản phẩm thành công.";
        public const string SuccessfullyUpdatedProductGroup = "Cập nhật nhóm sản phẩm thành công.";
        public const string SuccessfullyDeletedProductGroup = "Xóa nhóm sản phẩm thành công.";
        public const string ProductGroupInUse = "Nhóm sản phẩm đang được sử dụng, không thể xóa.";
        public const string ProductGroupRetrievedSuccessfully = "Lấy danh sách nhóm sản phẩm.";
        public const string ProductGroupDeleteFailedNotInactive = "Chỉ có thể xóa nhóm sản phẩm ở trạng thái không hoạt động.";
        public const string ProductGroupUpdateStatusFailedProductActive = "Không thể cập nhật trạng thái nhóm sản phẩm vì vẫn còn sản phẩm đang hoạt động thuộc nhóm này.";
    }
}