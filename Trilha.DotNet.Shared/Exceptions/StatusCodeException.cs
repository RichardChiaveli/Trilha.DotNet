namespace Trilha.DotNet.Shared.Exceptions;

public class StatusCodeException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode => statusCode;
}
