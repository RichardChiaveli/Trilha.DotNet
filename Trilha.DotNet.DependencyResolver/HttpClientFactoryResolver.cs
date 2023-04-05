using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace Trilha.DotNet.DependencyResolver
{
    public static class HttpClientFactoryResolver
    {
        public static IServiceCollection AddHttpClientFactory(this IServiceCollection services, IReadOnlyDictionary<string, string> clientSettings)
        {
            foreach (var setting in clientSettings)
            {
                services.AddHttpClient(setting.Key, (_, config) =>
                {
                    if (!string.IsNullOrWhiteSpace(setting.Value))
                        config.BaseAddress = new Uri(setting.Value);

                    config.Timeout = Timeout.InfiniteTimeSpan;
                    config.DefaultRequestHeaders.Clear();
                    config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            }

            return services;
        }
    }
}
