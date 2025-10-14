using ec_project_api.Services;

namespace ec_project_api.Helpers
{
    public static class EmailHelper
    {
        public static async Task<bool> SafeSendEmailAsync(
            CustomEmailService emailService, string email, string subject, string body)
        {
            try
            {
                await emailService.SendEmailAsync(email, subject, body);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static (string Subject, string Body) BuildVerificationEmail(string username, string verifyUrl)
        {
            var subject = "Xác nhận tài khoản EC Project";
            var body = $@"
                <h3>Xin chào {username},</h3>
                <p>Vui lòng xác nhận tài khoản của bạn bằng cách nhấn vào liên kết dưới đây:</p>
                <p><a href='{verifyUrl}' style='color:#2d89ef;font-weight:bold;'>Xác nhận tài khoản</a></p>
                <p>Liên kết này sẽ hết hạn sau <b>5 phút</b>.</p>";
            return (subject, body);
        }

        public static (string Subject, string Body) BuildResetPasswordEmail(string username, string resetUrl)
        {
            var subject = "Đặt lại mật khẩu EC Project";
            var body = $@"
                <h3>Xin chào {username},</h3>
                <p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào liên kết dưới đây để tiếp tục:</p>
                <p><a href='{resetUrl}' style='color:#2d89ef;font-weight:bold;'>Đặt lại mật khẩu</a></p>
                <p>Liên kết này sẽ hết hạn sau <b>5 phút</b>.</p>";
            return (subject, body);
        }
    }
}
