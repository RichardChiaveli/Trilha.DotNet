namespace Trilha.DotNet.DependencyResolver;

public static class ExceptionHandlerResolver
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature != null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;

                    dynamic json = new ExpandoObject();
                    json.Message = "An error occurred whilst processing your request";

                    var error = exceptionHandlerFeature.Error;
                    var isStatusCodeException = error is StatusCodeException;

                    if (isStatusCodeException)
                    {
                        var statusCodeException = (StatusCodeException)error;
                        context.Response.StatusCode = (int)statusCodeException.StatusCode;
                        json.Message = statusCodeException.Message;
                    }

                    if (!env.IsProduction())
                    {
                        json.Detail = error;
                    }

                    var stringSerializer = ((object)json).Stringify();
                    await context.Response.WriteAsync(stringSerializer);
                }
            });
        });
    }
}