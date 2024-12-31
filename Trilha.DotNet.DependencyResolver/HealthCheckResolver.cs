namespace Trilha.DotNet.DependencyResolver;

public static class HealthCheckResolver
{
    /// <summary>
    /// Health Check's: https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#Health-Checks
    /// </summary>
    /// <param name="services"></param>
    /// <param name="healthchecks"></param>
    /// <returns></returns>
    public static IServiceCollection AddHealthCheck(
        this IServiceCollection services
        , params KeyValuePair<string, IHealthCheck>[] healthchecks)
    {
        var plugin = services.AddHealthChecks();

        foreach (var healthcheck in healthchecks)
        {
            plugin.AddCheck(healthcheck.Key, healthcheck.Value);
        }

        services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(5);
            options.MaximumHistoryEntriesPerEndpoint(10);
            options.AddHealthCheckEndpoint("Health Checks", "/health");
        });

        return services;
    }

    public static IApplicationBuilder UseHealthCheck(
        this IApplicationBuilder app
        , params string[] healthchecks)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            var options = new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var result =
                        new
                        {
                            Data = report.Entries.Select(e => new
                            {
                                Nome = e.Key.ToUpper(),
                                Valido = e.Value.Status == HealthStatus.Healthy,
                                Mensagem = e.Value.Description
                            })
                        }.Stringify();

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync(result);
                }
            };

            endpoints.MapHealthChecks("/dev", options);

            foreach (var healthcheck in healthchecks)
            {
                endpoints.MapHealthChecks(healthcheck, options);
            }
        })
        .UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        })
        .UseHealthChecksUI(options =>
        {
            options.UIPath = "/monitor";
        });

        return app;
    }
}