namespace ec_project_api.Constants.messages
{
    public static class JwtMessages
    {
        public const string JwtSecretNotConfigured = "JWT Secret chưa được cấu hình.";
        public const string JwtExpirationNotConfigured = "Thời gian hết hạn JWT chưa được cấu hình.";
        public const string JwtRefreshExpirationNotConfigured = "Thời gian hết hạn Refresh Token chưa được cấu hình.";
        public const string JwtIssuerNotConfigured = "Không tìm thấy cấu hình Issuer cho JWT.";
        public const string JwtAudienceNotConfigured = "Không tìm thấy cấu hình Audience cho JWT.";
        public const string TokenExpired = "Token đã hết hạn.";
        public const string InvalidSignature = "Chữ ký token không hợp lệ.";
        public const string TokenNotProvided = "Token không được cung cấp. Vui lòng đăng nhập.";
        public const string InvalidToken = "Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.";
    }
}