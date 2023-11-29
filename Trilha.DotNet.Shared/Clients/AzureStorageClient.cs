namespace Trilha.DotNet.Shared.Clients;

public sealed class AzureStorageClient
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureStorageClient(string key, string blobContainerName)
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

        var client = new BlobServiceClient(key, blobOptions);

        _blobContainerClient = client.GetBlobContainerClient(blobContainerName);
    }

    public async Task<BlobDownloadInfo> DownloadAsync(string blobName)
        => await _blobContainerClient.GetBlobClient(blobName).DownloadAsync().ConfigureAwait(false);

    public async Task<int> AddOrUpdateAsync(IFormFile file)
    {
        BlobUploadOptions upOptions = new()
        {
            TransferOptions = new StorageTransferOptions
            {
                MaximumTransferSize = long.MaxValue,
                InitialTransferSize = long.MaxValue,
                InitialTransferLength = int.MaxValue,
                MaximumConcurrency = int.MaxValue,
                MaximumTransferLength = int.MaxValue
            },
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        };

        var result =
            await _blobContainerClient.GetBlobClient(file.FileName).UploadAsync(file.OpenReadStream(), upOptions).ConfigureAwait(false);

        return result.GetRawResponse().Status;
    }

    public async Task<bool> DeleteAsync(string blobName) =>
        await _blobContainerClient.GetBlobClient(blobName).DeleteIfExistsAsync();

    public async Task<List<BlobItem>> GetAsync(string filter)
    {
        var result = new List<BlobItem>();

        await foreach (var file in _blobContainerClient.GetBlobsAsync(prefix: filter).ConfigureAwait(false))
        {
            result.Add(file);
        }

        return result;
    }
}