namespace ec_project_api.Constants.messages
{
    public static class LocationMessages
    {
        public const string InvalidProvinceId = "ID tỉnh/thành phố không hợp lệ.";
        public const string InvalidWardId = "ID phường/xã không hợp lệ.";
        public const string ProvinceNotFound = "Không tìm thấy tỉnh.";
        public const string WardNotFound = "Không tìm thấy phường/xã.";
        public const string WardDoesNotBelongToProvince = "Phường/xã không thuộc về tỉnh đã chỉ định.";
        public const string ProvinceIdRequired = "Trường ProvinceId là bắt buộc.";
        public const string WardIdRequired = "Trường WardId là bắt buộc.";
        public const string GetProvincesSuccess = "Lấy danh sách tỉnh thành công.";
        public const string GetWardsSuccess = "Lấy danh sách phường/xã thành công.";
    }
}