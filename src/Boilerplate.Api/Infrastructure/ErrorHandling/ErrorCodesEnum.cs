using System.ComponentModel;

namespace Boilerplate.Api.Infrastructure.ErrorHandling;

/// <summary>
/// Error messages to use when business rules are broken.
/// These should be encapsulated in an exception - for instance the BusinessRuleException.
/// </summary>
public enum ErrorCodesEnum
{
    // ==== GENERIC 0-10000 ============
    [Description("An unhandled error occured. Staff have been notified")]
    GENERIC_INTERNAL = 101000,
    [Description("Unable to process request. Validation of properties failed")]
    GENERIC_VALIDATION = 102000,
    [Description("Client is not authorized")]
    GENERIC_UNAUTHORIZED = 103000,
    [Description("Client is forbidden.")]
    GENERIC_FORBIDDEN = 103100,
    [Description("The property {0} is in an incorrect format")]
    GENERIC_PROPERTY_BAD_FORMAT = 105000,

    // ==== Account 10000-20000 ============
    [Description("Account id does not exist")]
    ACCOUNT_ID_DOESNT_EXIST = 20101,
    [Description("No login has been created for the provided email")]
    ACCOUNT_LOGIN_PASSWORD_NOT_CREATED = 20102,
    [Description("The provided password is invalid")]
    ACCOUNT_LOGIN_PASSWORD_INVALID = 20103,
    [Description("The reset password token is invalid. Might have already been used")]
    ACCOUNT_RESETPASSWORD_TOKEN_INVALID = 20201,
    [Description("The provided email already exists and cannot be used")]
    ACCOUNT_EMAIL_ALREADY_EXIST = 20301,
    [Description("The provided email doesnt exist")]
    ACCOUNT_EMAIL_DOESNT_EXIST = 20302
}