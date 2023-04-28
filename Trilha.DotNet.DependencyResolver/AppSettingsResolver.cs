namespace Trilha.DotNet.DependencyResolver;

public static class AppSettingsResolver
{
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfigurationSection section)
    {
        var appSettings = new Dictionary<string, string>();
        new ConfigureFromConfigurationOptions<Dictionary<string, string>>(section).Configure(appSettings);
        services.AddSingleton(appSettings);

        return services;
    }
}
