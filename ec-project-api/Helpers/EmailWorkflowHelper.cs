using ec_project_api.Models;
using ec_project_api.Services;

namespace ec_project_api.Helpers
{
    public static class EmailWorkflowHelper
    {
        public static async Task<bool> SendEmailWithTokenAsync(
            CustomEmailService emailService,
            JwtService jwtService,
            User user,
            string baseUrl,
            EmailType type)
        {
            var token = jwtService.GenerateEmailVerificationToken(user.Email);
            var actionUrl = type switch
            {
                EmailType.Verification => UrlBuilderHelper.BuildVerificationUrl(baseUrl, token),
                EmailType.PasswordReset => UrlBuilderHelper.BuildResetUrl(baseUrl, token),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            var (subject, body) = type switch
            {
                EmailType.Verification => EmailHelper.BuildVerificationEmail(user.Username, actionUrl),
                EmailType.PasswordReset => EmailHelper.BuildResetPasswordEmail(user.Username, actionUrl),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return await EmailHelper.SafeSendEmailAsync(emailService, user.Email, subject, body);
        }
    }

    public enum EmailType
    {
        Verification,
        PasswordReset
    }
}
