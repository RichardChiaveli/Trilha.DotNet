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

        Initialize();
    }

    private void Initialize()
    {
        try
        {
            _host = _host.Replace("ftps://", "ftp://");

            if (!_host.Contains("ftp://"))
                _host = $"ftp://{_host}";

            var uri = new Uri(_host);
            var host = uri.Host;
            var resource = uri.AbsolutePath;

            _client = new FtpClient(host, _login, _password);
            if (_ssl)
            {
                _client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
                _client.Config.ValidateAnyCertificate = true;
            }

            if (_port != 21)
                _client.Port = _port;

            _client.Connect();

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
    }

    public FileTransferProtocolClient GetAll(out List<string> files)
    {
        files = new List<string>();
        if (!_isConnected) return this;

        files = _client?.GetListing()?.Select(i => i.FullName).ToList() ?? new List<string>();
        return this;
    }

    public FileTransferProtocolClient Download(List<string> files, out List<KeyValuePair<string, byte[]?>> fileInfo)
    {
        fileInfo = new List<KeyValuePair<string, byte[]?>>();
        if (!_isConnected) return this;
        byte[]? data = null;

        foreach (var file in files)
        {
            _client?.DownloadBytes(out data, file);
            fileInfo.Add(new KeyValuePair<string, byte[]?>(file, data));
        }

        return this;
    }

    public FileTransferProtocolClient Delete(IEnumerable<string> files)
    {
        if (!_isConnected) return this;

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _client?.Disconnect();

        if (!_client?.IsDisposed ?? false)
            _client?.Dispose();
    }
}
