namespace Application.Core
{
    public class AppException(int statusCode, string message, string details = null!)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Message { get; set; } = message;
        public string Details { get; set; } = details;
    }
}
