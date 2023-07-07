namespace Trilha.DotNet.DependencyResolver;

public static class BaseResolver
{
    public static IServiceCollection AddBase(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddCors()
            .AddMemoryCache()
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry()
            .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) =>
            {
                module.DisableDiagnosticSourceInstrumentation = false;
                module.EnableSqlCommandTextInstrumentation = true;
            })
            .AddHsts(opt =>
            {
                opt.MaxAge = DateTime.Now.AddYears(1) - DateTime.Now;
                opt.IncludeSubDomains = true;
            })
            .Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            })
            .AddScoped<AspNetUserComponent>()
            .AddScoped<NotifierComponent>()
            .AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()))
            .AddDetection();

        services
            .AddOcelot(configuration)
            .AddPolly();

        return services;
    }

    public static IApplicationBuilder UseBase(this IApplicationBuilder app)
    {
        app.UseDefaultFiles()
           .UseStaticFiles()
           .UseRouting()
           .UseHttpsRedirection()
           .UseCors(
               options => options
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetIsOriginAllowed(_ => true)
                   .AllowCredentials()
            )
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            })
            .UseRateLimiter()
            .UseOcelot().Wait();

        return app;
    }
}