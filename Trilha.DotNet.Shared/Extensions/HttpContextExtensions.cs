namespace Trilha.DotNet.Shared.Extensions;

public static class HttpContextExtensions
{
    public static string ExtractFromHeader(this HttpContext context, string param)
    {
        try
        {
            var request = context.Request;
            return request.ExtractFromHeader(param);
        }
        catch (Exception)
        {
            // ignored
        }

        return string.Empty;
    }

    public static string ExtractFromQueryString(this HttpContext context, string param)
    {
        try
        {
            var request = context.Request;
            return request.ExtractFromQueryString(param);
        }
        catch (Exception)
        {
            // ignored
        }

        return string.Empty;
    }

    public static T ExtractService<T>(this HttpContext? context) =>
        context!.RequestServices.GetService<T>()!;
}
