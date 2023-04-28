namespace Trilha.DotNet.DependencyResolver;

public static class CorsResolver
{
    public static IApplicationBuilder UseCorsAllowAny(this IApplicationBuilder app)
    {
        app.UseCors(
            options => options
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials()
        );

        return app;
    }
}
