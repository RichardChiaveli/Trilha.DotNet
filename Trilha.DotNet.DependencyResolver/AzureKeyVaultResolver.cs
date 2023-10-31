namespace Trilha.DotNet.DependencyResolver;

public static class AzureKeyVaultResolver
{
    public static IServiceCollection AddAzureKeyVault(this IServiceCollection services, IConfiguration configuration)
    {
        var tenantId = configuration["TENANT_ID"] ?? Environment.GetEnvironmentVariable("TENANT_ID");
        var clientId = configuration["CLIENT_ID"] ?? Environment.GetEnvironmentVariable("CLIENT_ID");
        var clientSecret = configuration["CLIENT_SECRET"] ?? Environment.GetEnvironmentVariable("CLIENT_SECRET");
        var keyVault = configuration["KEY_VAULT"] ?? Environment.GetEnvironmentVariable("KEY_VAULT");

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddSecretClient(new Uri(keyVault!));
            clientBuilder.UseCredential(new ClientSecretCredential(tenantId, clientId, clientSecret));
        });

        return services;
    }
}