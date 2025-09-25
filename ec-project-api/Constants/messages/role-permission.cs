namespace ec_project_api.Constants.Messages
{
    public static class RoleMessages
    {
        public const string RoleNotFound = "Không tìm thấy vai trò";
        public const string RoleCreated = "Vai trò đã được tạo thành công";
        public const string RoleUpdated = "Vai trò đã được cập nhật thành công";
        public const string RoleDeleted = "Vai trò đã được xóa thành công";
        public const string InvalidRoleData = "Dữ liệu vai trò không hợp lệ";
        public const string RoleAlreadyExists = "Vai trò đã tồn tại";
        public const string RoleNotFoundOrPermissionsInvalid = "Không tìm thấy vai trò hoặc quyền không hợp lệ";

    }
    public static class PermissionMessages
    {
        public const string PermissionsNotFound = "Không tìm thấy các quyền sau: {0}";
    }
}