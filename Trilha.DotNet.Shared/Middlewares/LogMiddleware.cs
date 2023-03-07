namespace Trilha.DotNet.Shared.Middlewares;

public abstract class LogMiddleware
{
    private const int ReadChunkBufferLength = 4096;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    private readonly RequestDelegate _next;

    protected LogMiddleware(RequestDelegate next)
    {
        _next = next;
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var originalBody = context.Response.Body;

            context.Request.EnableBuffering();

            await using var newResponseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = newResponseBody;

            var requestString = await HandleRequestAsync(context, newResponseBody);

            await _next(context);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalBody);

            newResponseBody.Seek(0, SeekOrigin.Begin);

            var responseString = HandleResponse(context, newResponseBody);

            await AddLogInfo(context, requestString, responseString);
        }
        catch (Exception e)
        {
            await AddLogError(context, e);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task<Dictionary<string, string>> HandleRequestAsync(
        HttpContext context, Stream requestStream)
    {
        var request = context.Request;
        var form = HandleRequestForm(request);
        var body = await HandleRequestBodyAsync(request, requestStream);

        return new Dictionary<string, string>
        {
            {"Schema", request.Scheme},
            {"Host", request.Host.Value},
            {"Path", request.Path},
            {"QueryString", request.QueryString.Value},
            {"RequestForm", form},
            {"RequestBody", body}
        };
    }

    private static async Task<string> HandleRequestBodyAsync(HttpRequest request, Stream requestStream)
    {
        await request.Body.CopyToAsync(requestStream);
        request.Body.Seek(0, SeekOrigin.Begin);
        return ReadStreamInChunks(requestStream);
    }

    private static string HandleRequestForm(HttpRequest request)
    {
        if (request.ContentType?.Contains("multipart/form-data") ?? false)
            return request.Form.Keys.ToDictionary(k => k, v => request.Form[v].ToString()).Stringify();

        return string.Empty;
    }

    private static Dictionary<string, string> HandleResponse(HttpContext context, Stream newResponseBody)
    {
        var request = context.Request;
        var response = context.Response;

        return new Dictionary<string, string>
        {
            {"StatusCode", response.StatusCode.ToString()},
            {"Schema", request.Scheme},
            {"Host", request.Host.Value},
            {"Path", request.Path},
            {"QueryString", request.QueryString.Value},
            {"ResponseBody", ReadStreamInChunks(newResponseBody)}
        };
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception e)
    {
        var jsonResult = new JsonResult(new
        {
            Error = HandleFormatException(e)
        })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };

        var routeData = context.GetRouteData();
        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(context, routeData, actionDescriptor);

        jsonResult.ContentType = "application/problem+json";

        await jsonResult.ExecuteResultAsync(actionContext);
    }

    private static string HandleFormatException(Exception e) =>
        e switch
        {
            StatusCodeException degustOneException => degustOneException.Message,
            _ => "Ops! Algo deu errado."
        };

    private static string ReadStreamInChunks(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[ReadChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        var result = textWriter.ToString();

        return result;
    }

    public abstract Task AddLogInfo(HttpContext context, Dictionary<string, string> request, Dictionary<string, string> response);

    public abstract Task AddLogError(HttpContext context, Exception e);
}
