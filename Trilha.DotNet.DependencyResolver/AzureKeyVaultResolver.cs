﻿namespace Trilha.DotNet.DependencyResolver;

public static class AzureKeyVaultResolver
{
    public static IServiceCollection AddAzureKeyVault(this IServiceCollection services, IConfiguration configuration)
    {
        var tenantId = configuration.GetValue<string>("TENANT_ID") ?? Environment.GetEnvironmentVariable("TENANT_ID");
        var clientId = configuration.GetValue<string>("CLIENT_ID") ?? Environment.GetEnvironmentVariable("CLIENT_ID");
        var clientSecret = configuration.GetValue<string>("CLIENT_SECRET") ?? Environment.GetEnvironmentVariable("CLIENT_SECRET");
        var keyVault = configuration.GetValue<string>("KEY_VAULT") ?? Environment.GetEnvironmentVariable("KEY_VAULT");

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddSecretClient(new Uri(keyVault!));
            clientBuilder.UseCredential(new ClientSecretCredential(tenantId, clientId, clientSecret));
        });

        var serviceProvider = services.BuildServiceProvider();
        var secretClient = serviceProvider.GetRequiredService<SecretClient>();
        var allSecrets = secretClient.GetPropertiesOfSecrets();

        var secrets = allSecrets.Select(secretProperty =>
                secretClient.GetSecret(secretProperty.Name))
            .ToDictionary(response => response.Value.Name, response => response.Value.Value);

        services.AddSingleton(secrets);

        return services;
    }
}