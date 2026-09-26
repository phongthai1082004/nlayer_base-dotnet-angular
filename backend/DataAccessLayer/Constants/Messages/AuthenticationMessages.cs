namespace DataAccessLayer.Constants.Messages
{
    public static class AuthenticationMessages
    {
        public const string UserNotFound = "User not found.";
        // Email
        public const string EmailRequired = "Email cannot be empty.";
        public const string EmailInvalidFormat = "Invalid email format.";
        public const string EmailMaxLength = "Email cannot exceed 100 characters.";
        public const string EmailExists = "A user with this email already exists.";

        // Authentication
        public const string PasswordRequired = "Password cannot be empty.";
        public const string PasswordLength = "Password must be between 8 and 100 characters.";
        public const string PasswordComplex = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
        public const string PasswordMismatch = "Password does not match.";

        // Token
        public const string InvalidRefreshToken = "Invalid refresh token.";

    }
}
