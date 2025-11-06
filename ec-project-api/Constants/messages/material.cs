namespace ec_project_api.Constants.messages
{
    public static class MaterialMessages
    {
        public const string MaterialNotFound = "Không tìm thấy chất liệu.";
        public const string MaterialAlreadyExists = "Chất liệu đã tồn tại.";
        public const string MaterialNameAlreadyExists = "Tên chất liệu đã tồn tại.";
        public const string MaterialCreateFailed = "Tạo chất liệu thất bại.";
        public const string MaterialUpdateFailed = "Cập nhật chất liệu thất bại.";
        public const string MaterialDeleteFailed = "Xóa chất liệu thất bại.";
        public const string MaterialDeleteFailedNotInActive = "Chỉ có thể xóa chất liệu ở trạng không hoạt động.";
        public const string InvalidMaterialData = "Dữ liệu chất liệu không hợp lệ.";
        public const string InvalidMaterialId = "ID chất liệu không hợp lệ.";
        public const string SuccessfullyCreatedMaterial = "Tạo chất liệu thành công.";
        public const string SuccessfullyUpdatedMaterial = "Cập nhật chất liệu thành công.";
        public const string SuccessfullyDeletedMaterial = "Xóa chất liệu thành công.";
        public const string MaterialInUse = "Chất liệu đang được sử dụng, không thể xóa.";
        public const string MaterialUpdateStatusFailedProductActive = "Không thể cập nhật trạng thái chất liệu vì vẫn còn sản phẩm đang hoạt động thuộc chất liệu này.";

    }
}