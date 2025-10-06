namespace ec_project_api.Constants.Messages
{
    public static class AuthMessages
    {
        public const string RegisterSuccessful = "Đăng ký tài khoản thành công.";
        public const string RegisterFailed = "Đăng ký tài khoản thất bại.";
        public const string UserAlreadyExists = "Tên đăng nhập hoặc email đã tồn tại.";

        public const string LoginSuccessful = "Đăng nhập thành công.";
        public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không chính xác.";
        public const string AccountNotVerified = "Tài khoản chưa được xác minh. Vui lòng kiểm tra email của bạn.";
        public const string AccountLocked = "Tài khoản của bạn đã bị khóa.";

        public const string VerificationEmailSent = "Email xác nhận đã được gửi thành công.";
        public const string VerificationSuccess = "Tài khoản của bạn đã được xác nhận thành công!";
        public const string AlreadyVerified = "Tài khoản đã được xác minh trước đó.";
        public const string InvalidVerificationLink = "Liên kết xác nhận không hợp lệ hoặc đã hết hạn.";
        public const string EmailNotFoundInToken = "Không tìm thấy thông tin email trong token.";
        public const string UserNotFound = "Người dùng không tồn tại.";
        public const string VerificationExpired = "Liên kết xác nhận đã hết hạn. Vui lòng yêu cầu gửi lại email xác nhận.";

        public const string TokenRefreshed = "Làm mới token thành công.";
        public const string InvalidRefreshToken = "Refresh token không hợp lệ.";
        public const string ExpiredRefreshToken = "Refresh token đã hết hạn.";
        public const string RevokedRefreshToken = "Refresh token đã bị thu hồi.";

        public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau.";
    }
}
