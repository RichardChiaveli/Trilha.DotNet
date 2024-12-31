namespace Trilha.DotNet.DependencyResolver;

public static class AzureServiceBusSdkResolver
{
    public static IServiceCollection AddAzureServiceBusSdk(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING");

        services.AddSingleton(_ => new ServiceBusClient(connectionString));

        return services;
    }
}