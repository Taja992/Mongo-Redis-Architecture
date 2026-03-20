namespace API.Core.Domain.Exceptions;

public abstract class AppException(
    string message,
    string errorCode,
    int statusCode = 500,
    Exception? innerException = null,
    Dictionary<string, object>? extensions = null
) : Exception(message, innerException)
{
    public string ErrorCode { get; } = errorCode;
    public int StatusCode { get; } = statusCode;
    public Dictionary<string, object>? Extensions { get; } = extensions;
}
