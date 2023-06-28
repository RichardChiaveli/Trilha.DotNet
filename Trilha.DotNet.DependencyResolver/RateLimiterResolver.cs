namespace Trilha.DotNet.DependencyResolver;

public static class RateLimiterResolver
{
    public static IServiceCollection AddRateLimiterSetup(this IServiceCollection services, int limit)
    {
        services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = limit;
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 1;
            })
            .AddSlidingWindowLimiter("sliding", options =>
            {
                options.PermitLimit = limit;
                options.Window = TimeSpan.FromSeconds(10);
                options.SegmentsPerWindow = limit;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 1;
            }));

        return services;
    }

    public static IApplicationBuilder UseRateLimiterSetup(this IApplicationBuilder app)
    {
        app.UseRateLimiter();

        return app;
    }
}