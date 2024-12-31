namespace Trilha.DotNet.Shared.SDKs;

public sealed class SignalRSdk
{
    public HubConnection Connection { get; }
    public bool IsConnected => Connection.State == HubConnectionState.Connected;

    public SignalRSdk(string url, int limitAttempts = 0)
    {
        Connection = new HubConnectionBuilder().WithUrl(new Uri(url).AbsoluteUri).Build();

        Connection.Closed += async _ =>
        {
            await ConnectAsync(new CancellationToken(), limitAttempts);
        };

        ConnectAsync(new CancellationToken(), limitAttempts).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public async Task SendAsync<T>(string method, params object[] args)
    {
        foreach (var o in args)
        {
            Console.WriteLine(o);
        }

        await Connection.InvokeCoreAsync<T>(method, args);
    }

    public Task ReceiveAsync<T>(string method, Func<T, Task> messageHandler)
    {
        Connection.On(method, messageHandler);
        return Task.CompletedTask;
    }

    private async Task ConnectAsync(CancellationToken token, int limitAttempts = 0)
    {
        try
        {
            await Connection.StartAsync(token);
        }
        catch when (token.IsCancellationRequested)
        {
            await DisposeSignalRAsync(token);
        }
        catch (Exception)
        {
            if (limitAttempts > 0)
            {
                limitAttempts--;
                await ConnectAsync(token, limitAttempts);
            }
            else
            {
                await DisposeSignalRAsync(token);
            }
        }
    }

    private async Task DisposeSignalRAsync(CancellationToken token)
    {
        await Connection.DisposeAsync();
        await Task.Delay(5000, token);
    }
}
