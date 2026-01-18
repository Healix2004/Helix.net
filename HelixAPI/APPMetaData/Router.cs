using System;

namespace Helix.API.APPMetaData
{
    public static class Router
    {
        private const string Root = "api";
        private const string Version = "v1";
        public const string BaseUrl = $"{Root}/{Version}";

        public static class AuthRouting
        {
            private const string Prefix = $"{BaseUrl}/auth";

            // Authentication endpoints
            public const string Register = $"{Prefix}/register";
            public const string Login = $"{Prefix}/login";
            public const string ForgotPassword = $"{Prefix}/forgot-password";
            public const string ResetPassword = $"{Prefix}/reset-password";
            public const string ChangePassword = $"{Prefix}/change-password";

            // Email confirmation endpoints
            public const string ConfirmEmail = $"{Prefix}/confirm-email";
            public const string ConfirmEmailGet = $"{Prefix}/confirmemail";

            // User profile endpoints
            public const string GetUserProfile = $"{Prefix}/profile/{{userId}}";
            public const string GetUserByEmail = $"{Prefix}/user/{{email}}";

            // Route helper methods
            public static string GetUserProfileRoute(string userId) => $"{Prefix}/profile/{userId}";
            public static string GetUserByEmailRoute(string email) => $"{Prefix}/user/{email}";
            public static string ConfirmEmailGetRoute(Guid userId, string token) => 
                $"{Prefix}/confirmemail?userid={userId}&token={Uri.EscapeDataString(token)}";
        }
        public static class FileRouting
        {
            private const string Prefix = $"{BaseUrl}/file";

            // Collection endpoints
            public const string Upload = $"{Prefix}/upload";
            public const string UploadMultiple = $"{Prefix}/upload-multiple";

            // Single resource endpoints
            public const string Download = $"{Prefix}/download/{{*filePath}}";
            public const string Delete = $"{Prefix}/{{*filePath}}";
            public const string Exists = $"{Prefix}/exists/{{*filePath}}";

            // Route helper methods
            public static string DownloadRoute(string filePath) => $"{Prefix}/download/{Uri.EscapeDataString(filePath)}";
            public static string DeleteRoute(string filePath) => $"{Prefix}/{Uri.EscapeDataString(filePath)}";
            public static string ExistsRoute(string filePath) => $"{Prefix}/exists/{Uri.EscapeDataString(filePath)}";
        }

        public static class EmailRouting
        {
            private const string Prefix = $"{BaseUrl}/emails";

            // Collection endpoints
            public const string Send = $"{Prefix}/send";
        }
    }
}

