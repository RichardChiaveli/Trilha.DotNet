namespace Trilha.DotNet.DependencyResolver;

public static class RefitClientFactoryResolver
{
    public static IServiceCollection AddRefitResolver<T>(this IServiceCollection services, string endpoint) where T : class
    {
        services.AddRefitClient<T>()
            .ConfigureHttpClient(config =>
            {
                config.BaseAddress = new Uri(endpoint);
                config.Timeout = Timeout.InfiniteTimeSpan;
                
                config.DefaultRequestHeaders.Clear();
                config.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(_ => new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseProxy = false,
                Proxy = null,
                PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
                PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                MaxConnectionsPerServer = int.MaxValue
            });

        return services;
    }
}