namespace Trilha.DotNet.Shared.SDKs;

public class FTPSdk(string host, string login, string password, int port = 21, bool ssl = false) : IDisposable
{
    private FtpClient? _client;
    private bool _isConnected;
    private string _host = host;

    private Uri CreateUri()
    {
        if (!_host.Contains("://"))
            _host = $"ftp://{_host}";
        return new Uri(_host);
    }

    public FTPSdk List(out IEnumerable<string> files)
    {
        files = [];
        if (!_isConnected) return this;

        files = _client?.GetListing()?
            .Where(e => e.Type == FtpObjectType.File)
            .Select(i => i.FullName) ?? [];

        return this;
    }

    public FTPSdk Download(IEnumerable<string> files, out Dictionary<string, byte[]> fileInfo)
    {
        fileInfo = [];

        if (!_isConnected || !files.Any()) return this;
        byte[]? data = null;

        foreach (var file in files)
        {
            _client?.DownloadBytes(out data, file);

            if (data != null)
            {
                fileInfo.Add(file, data);
            }
        }

        return this;
    }

    public FTPSdk Delete(IEnumerable<string> files)
    {
        if (!_isConnected || !files.Any()) return this;

        foreach (var file in files)
        {
            _client?.DeleteFile(file);
        }

        return this;
    }

    public FTPSdk Upload(string filename, byte[] data)
    {
        if (_isConnected)
            _client?.UploadBytes(data, filename);

        return this;
    }

    public FTPSdk Connect()
    {
        try
        {
            var uri = CreateUri();
            var resource = uri.AbsolutePath;

            _client = new FtpClient(uri.Host, login, password);
            if (ssl)
            {
                _client.Config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                _client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                _client.Config.ValidateAnyCertificate = true;
                _client.ValidateCertificate += (_, e) =>
                {
                    e.Accept = e.PolicyErrors == System.Net.Security.SslPolicyErrors.None;
                };
            }

            if (port != 21)
                _client.Port = port;

            _client.AutoConnect();

            if (!string.IsNullOrWhiteSpace(resource) && resource != "/")
            {
                resource = WebUtility.UrlDecode(resource);
                _client.SetWorkingDirectory(resource);
            }

            _isConnected = _client.IsConnected;
        }
        catch
        {
            _isConnected = false;
        }

        return this;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _client?.Disconnect();
        _client?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
