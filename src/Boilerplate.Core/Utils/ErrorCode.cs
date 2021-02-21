namespace Boilerplate.Core.Utils
{
    public class ErrorCode {
        public ErrorCode(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }
        public string Message { get; set; }
    }
}