namespace Trilha.DotNet.Shared.Middlewares;

public abstract class LogMiddleware : IMiddleware
{
    private readonly RequestDelegate _next;

    protected LogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Request.EnableBuffering();

        var originalBodyStream = context.Response.Body;

        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        var request = new Dictionary<string, string>
        {
            {"Method", context.Request.Method},
            {"Url",$"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}"},
            {"QueryString", context.Request.QueryString.ToString()}
        };

        if (context.Request.Headers.Any())
        {
            var headers = context.Request.Headers.ToDictionary(header =>
                header.Key, header => header.Value.ToString());

            request["Headers"] = headers.Stringify();
        }

        if (context.Request.HasFormContentType)
        {
            var form = await context.Request.ReadFormAsync();

            var formData = form.ToDictionary(field =>
                field.Key, field => field.Value.ToString());

            request["FormData"] = formData.Stringify();
        }

        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            body = body.Replace("\r\n", "").Replace("\n", "").Replace(" ", "");

            if (!string.IsNullOrWhiteSpace(body))
            {
                request["Body"] = body;
            }

            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }

        await AddRequestLog(context, request);

        await _next(context);

        var response = new Dictionary<string, string>();

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        response["Body"] = responseBody;

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        await responseBodyStream.CopyToAsync(originalBodyStream);

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        response["IpAddress"] = ipAddress ?? string.Empty;

        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        response["ElapsedMilliseconds"] = elapsedMilliseconds.ToString();
        response["StatusCode"] = context.Response.StatusCode.ToString();

        await AddResponsetLog(context, response);
    }

    public abstract Task AddRequestLog(HttpContext context, Dictionary<string, string> args);
    public abstract Task AddResponsetLog(HttpContext context, Dictionary<string, string> args);
}
