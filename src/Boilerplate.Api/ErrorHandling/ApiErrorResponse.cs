using System.ComponentModel.DataAnnotations;

namespace Boilerplate.Api.ErrorHandling
{
    /// <summary>
    /// The typical error type that will be sent to the client when an error happens in the API
    /// </summary>
    public class ApiErrorResponse
    {
        public ApiErrorResponse(string message, string errorCode, ModelValidationError[] validationErrors = null)
        {
            Message = message;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors ?? new ModelValidationError[] { };
        }

        [Required]
        public string Message { get; set; }

        [Required]
        public string ErrorCode { get; set; }

        /// <summary>
        /// will contain validation errors if any
        /// </summary>
        [Required]
        public ModelValidationError[] ValidationErrors { get; set; } = new ModelValidationError[] { };
    }

    public class ModelValidationError
    {
        public ModelValidationError(string field, string description)
        {
            Field = field;
            Description = description;
        }

        [Required]
        public string Field { get; set; }

        [Required]
        public string Description { get; set; }
    }
}