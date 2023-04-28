namespace Trilha.DotNet.DependencyResolver;

public static class ResponseCompressionResolver
{
    public static IServiceCollection AddBrotliCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { MediaTypeNames.Application.Json });
            })
            .Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

        return services;
    }
}
