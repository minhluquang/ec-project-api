namespace ec_project_api.Constants.Messages
{
    public static class AuthMessages
    {
        // ================================
        // JWT CONFIGURATION ERRORS
        // ================================
        public const string JwtSecretNotConfigured = "JWT Secret chưa được cấu hình.";
        public const string JwtExpirationNotConfigured = "Thời gian hết hạn JWT chưa được cấu hình.";
        public const string JwtRefreshExpirationNotConfigured = "Thời gian hết hạn Refresh Token chưa được cấu hình.";
        public const string JwtIssuerNotConfigured = "Không tìm thấy cấu hình Issuer cho JWT.";
        public const string JwtAudienceNotConfigured = "Không tìm thấy cấu hình Audience cho JWT.";

        // ================================
        // TOKEN ERRORS
        // ================================
        public const string TokenNotProvided = "Token không được cung cấp. Vui lòng đăng nhập.";
        public const string InvalidOrExpiredToken = "Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.";
        public const string InvalidSignature = "Chữ ký token không hợp lệ.";

        // Refresh Token
        public const string MissingRefreshToken = "Refresh token không được để trống.";
        public const string InvalidRefreshToken = "Refresh token không hợp lệ.";
        public const string ExpiredRefreshToken = "Refresh token đã hết hạn.";

        // ================================
        // AUTHENTICATION & ACCOUNT
        // ================================
        public const string RegisterSuccessful = "Đăng ký tài khoản thành công.";
        public const string RegisterFailed = "Đăng ký tài khoản thất bại.";
        public const string UserAlreadyExists = "Tên đăng nhập hoặc email đã tồn tại.";

        public const string LoginSuccessful = "Đăng nhập thành công.";
        public const string PasswordResetSuccessful = "Đặt lại mật khẩu thành công.";
        public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không chính xác.";
        public const string AccountNotVerified = "Tài khoản chưa được xác minh. Vui lòng kiểm tra email của bạn.";
        public const string AccountInactive = "Tài khoản của bạn hiện không hoạt động. Vui lòng liên hệ quản trị viên.";
        public const string AccountLocked = "Tài khoản của bạn đã bị khóa.";

        // ================================
        // VERIFICATION EMAIL
        // ================================
        public const string VerificationEmailSent = "Email xác nhận đã được gửi thành công.";
        public const string VerificationSuccess = "Tài khoản của bạn đã được xác nhận thành công!";
        public const string AlreadyVerified = "Tài khoản đã được xác minh trước đó.";
        public const string InvalidVerificationLink = "Liên kết xác nhận không hợp lệ hoặc đã hết hạn.";
        public const string EmailNotFoundInToken = "Không tìm thấy thông tin email trong token.";
        public const string UserNotFound = "Người dùng không tồn tại.";
        public const string VerificationExpired = "Liên kết xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại email xác nhận.";

        // ================================
        // MISC
        // ================================
        public const string TokenRefreshed = "Làm mới token thành công.";
        public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau.";
    }
}
