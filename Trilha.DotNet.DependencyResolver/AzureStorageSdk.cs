namespace Trilha.DotNet.DependencyResolver;

public static class AzureStorageSdk
{
    public static IServiceCollection AddAzureStorageSdk(this IServiceCollection services, IConfiguration configuration)
    {
        BlobClientOptions blobOptions = new()
        {
            Transport = new HttpClientTransport(new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan
            }),
            Retry =
            {
                NetworkTimeout = Timeout.InfiniteTimeSpan
            }
        };

        var key = configuration.GetValue<string>("AZURE_STORAGE_KEY") ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_KEY");
        var container = configuration.GetValue<string>("AZURE_STORAGE_CONTAINER") ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONTAINER");

        var blobServiceClient = new BlobServiceClient(key, blobOptions);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);

        services.AddSingleton(_ => blobServiceClient);
        services.AddSingleton(_ => blobContainerClient);

        return services;
    }
}