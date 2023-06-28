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
                    context.Response.ContentType = "application/json";

                    dynamic json = new ExpandoObject();
                    json.Message = "An error occurred whilst processing your request";

                    if (!env.IsProduction())
                        json.Error = exceptionHandlerFeature.Error;

                    string stringSerializer = JsonConvert.SerializeObject(json);
                    await context.Response.WriteAsync(stringSerializer);
                }
            });
        });
    }
}
