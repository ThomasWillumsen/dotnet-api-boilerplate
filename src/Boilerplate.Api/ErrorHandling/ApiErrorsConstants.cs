namespace Boilerplate.Api.ErrorHandling
{
    /// <summary>
    /// Error messages sent from the API project as part of the response.
    /// These should not be edited because API clients might be dependant on them
    /// </summary>
    public static class ApiErrorsConstants
    {
        public const string MODEL_VALIDATION_ERROR = "One or more of the inputs did not meet the requirements";
        public const string NOT_FOUND = "NOT_FOUND";
        public const string INTERNAL_SERVER_ERROR = "Operation failed internally";
    }
}