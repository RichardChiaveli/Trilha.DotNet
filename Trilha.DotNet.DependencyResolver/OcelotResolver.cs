namespace Trilha.DotNet.DependencyResolver;

public static class OcelotResolver
{
    public static IServiceCollection AddOcelotSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOcelot(configuration).AddPolly();

        return services;
    }

    public static IApplicationBuilder UseOcelotSetup(this IApplicationBuilder app)
    {
        app.UseOcelot().Wait();

        return app;
    }
}