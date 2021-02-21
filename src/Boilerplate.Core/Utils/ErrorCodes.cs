namespace Boilerplate.Core.Utils
{
    /// <summary>
    /// Error messages to use when business rules are broken.
    /// These should be encapsulated in an exception - for instance the BusinessRuleException.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary></summary>
        public static int errorRoot = 100000;

        // ==== GENERIC 0-10000 ============
        public static ErrorCode INTERNAL = new ErrorCode(errorRoot + 1000, "An unhandled error occured. Staff have been notified");
        public static ErrorCode VALIDATION = new ErrorCode(errorRoot + 2000, "Unable to process request. Validation of properties failed");
        public static ErrorCode NOTFOUND = new ErrorCode(errorRoot + 3000, "Entity was not found based on provided identifier");
        public static ErrorCode UNAUTHORIZED = new ErrorCode(errorRoot + 4000, "Client is not authorized");
        public static ErrorCode PROPERTY_BAD_FORMAT = new ErrorCode(errorRoot + 9001, "The property {0} is in an incorrect format");
        public static ErrorCode FORBIDDEN = new ErrorCode(errorRoot + 4030, "Client is forbidden.");

        // ==== Example 10000-20000 ============
        #region Example
        public static int exampleRoot = errorRoot + 10000;
        public static ErrorCode EXAMPLE_SOME_RULE = new ErrorCode(exampleRoot + 1, "Some rule was broken");
        #endregion
    }
}