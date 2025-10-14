using ec_project_api.Constants.variables;

namespace ec_project_api.Helpers
{
    public static class UrlBuilderHelper
    {
        private static string BuildUrl(string baseUrl, string path, string token)
            => $"{baseUrl.TrimEnd('/')}/{path}?token={token}";

        public static string BuildVerificationUrl(string baseUrl, string token)
            => BuildUrl(baseUrl, $"{PathVariables.AuthRoot}/{PathVariables.Verify}", token);

        public static string BuildResetUrl(string baseUrl, string token)
            => BuildUrl(baseUrl, $"{PathVariables.AuthRoot}/{PathVariables.ResetPassword}", token);
    }
}
