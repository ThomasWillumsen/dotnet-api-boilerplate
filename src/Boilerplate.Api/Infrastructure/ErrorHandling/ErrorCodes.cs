namespace Boilerplate.Api.Infrastructure.ErrorHandling;

/// <summary>
/// Error messages to use when business rules are broken.
/// These should be encapsulated in an exception - for instance the BusinessRuleException.
/// </summary>
public static class ErrorCodes
{
    /// <summary></summary>
    private static int errorRoot = 100000;

    // ==== GENERIC 0-10000 ============
    public static ErrorCode INTERNAL = new ErrorCode(errorRoot + 1000, "An unhandled error occured. Staff have been notified");
    public static ErrorCode VALIDATION = new ErrorCode(errorRoot + 2000, "Unable to process request. Validation of properties failed");
    public static ErrorCode UNAUTHORIZED = new ErrorCode(errorRoot + 3000, "Client is not authorized");
    public static ErrorCode FORBIDDEN = new ErrorCode(errorRoot + 3100, "Client is forbidden.");
    public static ErrorCode PROPERTY_BAD_FORMAT = new ErrorCode(errorRoot + 5000, "The property {0} is in an incorrect format");

    // ==== Account 10000-20000 ============
    #region Account
    public static class Account
    {
        private static int accountRoot = errorRoot + 10000;
        public static ErrorCode LOGIN_EMAIL_DOESNT_EXIST = new ErrorCode(accountRoot + 101, "The provided email doesnt exist");
        public static ErrorCode LOGIN_PASSWORD_NOT_CREATED = new ErrorCode(accountRoot + 102, "No login has been created for the provided email");
        public static ErrorCode LOGIN_PASSWORD_INVALID = new ErrorCode(accountRoot + 103, "The provided password is invalid");
        public static ErrorCode RESETPASSWORD_TOKEN_INVALID = new ErrorCode(accountRoot + 201, "The reset password token is invalid. Might have already been used");
        public static ErrorCode ACCOUNT_EMAIL_ALREADY_EXIST = new ErrorCode(accountRoot + 301, "The provided email already exists and cannot be used");
    }
    #endregion
}