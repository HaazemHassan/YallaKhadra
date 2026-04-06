namespace YallaKhadra.Core.Bases {
    public static class ErrorCodes {

        public static class Authentication {
            public const string InvalidCredentials = "InvalidCredentials";
            public const string EmailNotConfirmed = "EmailNotConfirmed";
            public const string UserAlreadyExists = "UserAlreadyExists";
            public const string UserNotFound = "UserNotFound";
            public const string InvalidRefreshToken = "InvalidRefreshToken";
            public const string RefreshTokenExpired = "RefreshTokenExpired";
        }
    }
}
