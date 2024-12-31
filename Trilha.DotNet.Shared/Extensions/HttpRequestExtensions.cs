namespace Trilha.DotNet.Shared.Extensions;

public static class HttpRequestExtensions
{
    public static string ExtractFromHeader(this HttpRequest request, string param)
    {
        try
        {
            var existe = request.Headers.ContainsKey(param);

            if (existe)
                return request.Headers[param].ToString();
        }
        catch (Exception)
        {
            // ignored
        }

        return string.Empty;
    }

    public static string ExtractFromQueryString(this HttpRequest request, string param)
    {
        try
        {
            var existe = request.Query.ContainsKey(param);

            if (existe)
                return request.Query[param].ToString();
        }
        catch (Exception)
        {
            // ignored
        }

        return string.Empty;
    }
}
