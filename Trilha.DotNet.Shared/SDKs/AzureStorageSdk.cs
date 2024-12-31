namespace Trilha.DotNet.Shared.SDKs;

public class AzureStorageSdk(BlobContainerClient blobContainerClient)
{
    /// <summary>
    /// Baixa um arquivo do Blob Storage.
    /// </summary>
    protected async Task<BlobDownloadInfo> DownloadAsync(string blobName)
    {
        try
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            return await blobClient.DownloadAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error downloading blob '{blobName}'.", ex);
        }
    }

    /// <summary>
    /// Adiciona ou atualiza um arquivo no Blob Storage.
    /// </summary>
    protected async Task<int> AddOrUpdateAsync(IFormFile file)
    {
        try
        {
            var blobClient = blobContainerClient.GetBlobClient(file.FileName);

            var uploadOptions = new BlobUploadOptions
            {
                TransferOptions = new StorageTransferOptions
                {
                    MaximumTransferSize = long.MaxValue,
                    InitialTransferSize = long.MaxValue,
                    MaximumConcurrency = int.MaxValue
                },
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(), uploadOptions).ConfigureAwait(false);
            return result.GetRawResponse().Status;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error adding or updating file '{file.FileName}' in Blob Storage.", ex);
        }
    }

    /// <summary>
    /// Deleta um arquivo do Blob Storage.
    /// </summary>
    protected async Task<bool> DeleteAsync(string blobName)
    {
        try
        {
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync().ConfigureAwait(false);
            return response.Value;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error deleting blob '{blobName}'.", ex);
        }
    }

    /// <summary>
    /// Obtém uma lista de blobs a partir de um filtro.
    /// </summary>
    protected async Task<List<BlobItem>> GetAsync(string filter)
    {
        var result = new List<BlobItem>();

        try
        {
            await foreach (var file in blobContainerClient.GetBlobsAsync(prefix: filter).ConfigureAwait(false))
            {
                result.Add(file);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error listing blobs with the provided filter.", ex);
        }

        return result;
    }
}