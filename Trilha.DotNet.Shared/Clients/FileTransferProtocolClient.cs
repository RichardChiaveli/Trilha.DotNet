namespace Trilha.DotNet.Shared.Clients;

public class FileTransferProtocolClient : IDisposable
{
    private string _host;
    private FtpClient? _client;
    private bool _isConnected;

    private readonly string _login;
    private readonly string _password;
    private readonly int _port;
    private readonly bool _ssl;

    public FileTransferProtocolClient(string host, string login, string password, int port = 21, bool ssl = false)
    {
        _host = host;
        _login = login;
        _password = password;
        _port = port;
        _ssl = ssl;
    }

    private Uri CreateUri()
    {
        if (!_host.Contains("://"))
            _host = $"ftp://{_host}";
        return new Uri(_host);
    }

    public FileTransferProtocolClient List(out List<string> files)
    {
        files = new List<string>();
        if (!_isConnected) return this;

        files = _client?.GetListing()?
            .Where(e => e.Type == FtpObjectType.File)
            .Select(i => i.FullName)
            .ToList() ?? new List<string>();

        return this;
    }

    public FileTransferProtocolClient Download(IList<string> files, out IList<KeyValuePair<string, byte[]>> fileInfo)
    {
        fileInfo = new List<KeyValuePair<string, byte[]>>();

        if (!_isConnected || !files.Any()) return this;
        byte[]? data = null;

        foreach (var file in files)
        {
            _client?.DownloadBytes(out data, file);

            if (data != null)
            {
                fileInfo.Add(new KeyValuePair<string, byte[]>(file, data));
            }
        }

        return this;
    }

    public FileTransferProtocolClient Delete(IList<string> files)
    {
        if (!_isConnected || !files.Any()) return this;

        foreach (var file in files)
        {
            _client?.DeleteFile(file);
        }

        return this;
    }

    public FileTransferProtocolClient Upload(string filename, byte[] data)
    {
        if (_isConnected)
            _client?.UploadBytes(data, filename);

        return this;
    }

    public FileTransferProtocolClient Connect()
    {
        try
        {
            var uri = CreateUri();
            var host = uri.Host;
            var resource = uri.AbsolutePath;

            _client = new FtpClient(host, _login, _password);
            if (_ssl)
            {
                _client.Config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                _client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                _client.Config.ValidateAnyCertificate = true;
                _client.ValidateCertificate += (_, e) =>
                {
                    e.Accept = e.PolicyErrors == System.Net.Security.SslPolicyErrors.None;
                };
            }

            if (_port != 21)
                _client.Port = _port;

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
