using System;

namespace Boilerplate.Core.Utils.Exceptions
{
    /// <summary>
    /// To be thrown when business logic fails.
    /// This exception type is caught by the global filter and formatted before sending responses to the client.
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string errorCode, string reasonText)
        {
            ErrorCode = errorCode;
            ReasonText = reasonText;
        }

        public string ErrorCode { get; set; }
        public string ReasonText { get; set; }
    }
}