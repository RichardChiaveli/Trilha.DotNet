namespace Trilha.DotNet.Shared.Clients;

public sealed class AzureStorageClient
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureStorageClient(string key, string blobContainerName)
    {
        var client = new BlobServiceClient(key);
        _blobContainerClient = client.GetBlobContainerClient(blobContainerName);
    }

    public async Task<BlobDownloadInfo> DownloadAsync(string blobName)
        => await _blobContainerClient.GetBlobClient(blobName).DownloadAsync().ConfigureAwait(false);

    public async Task<int> AddOrUpdateAsync(string blobName, Stream data)
    {
        var result =
            await _blobContainerClient.GetBlobClient(blobName).UploadAsync(data,
                new BlobHttpHeaders
                {
                    ContentType = blobName.GetContentType()
                }).ConfigureAwait(false);

        return result.GetRawResponse().Status;
    }

    public async Task<int> AddOrUpdateAsync(IFormFile file)
        => await AddOrUpdateAsync(file.FileName, file.OpenReadStream());

    public Task AddOrUpdateRangeAsync(Dictionary<string, Stream> arg)
    {
        var poolTasks = new List<Task>();

        foreach (var (key, value) in arg)
        {
            poolTasks.Add(AddOrUpdateAsync(key, value));
        }

        poolTasks.PromisseAllAsync();

        return Task.CompletedTask;
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