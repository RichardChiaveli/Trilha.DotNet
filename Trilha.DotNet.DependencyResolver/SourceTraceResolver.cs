namespace Trilha.DotNet.DependencyResolver;

public static class SourceTraceResolver
{
    public static IServiceCollection AddSourceTrace(IServiceCollection services)
    {
        services.AddDetection();

        return services;
    }
}