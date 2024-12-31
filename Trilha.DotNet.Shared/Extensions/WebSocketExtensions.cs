namespace Trilha.DotNet.Shared.Extensions;

public static class WebSocketExtensions
{
    private const int BufferSize = 1024;

    public static async Task SendObjectAsync<T>(this WebSocket socket, T payload)
    {
        var bytes = Encoding.UTF8.GetBytes(payload!.Stringify());
        var buffer = new ArraySegment<byte>(bytes);

        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public static async Task<T> ReceiveObjectAsync<T>(this WebSocket socket)
    {
        var buffer = new byte[BufferSize];
        var segment = new ArraySegment<byte>(buffer);

        var response = await socket.ReceiveAsync(segment, CancellationToken.None);

        var value = Encoding.UTF8.GetString(buffer, 0, response.Count);

        return value.ParseJson<T>();
    }

    public static async Task CloseAsync(this WebSocket socket)
    {
        if (socket.CloseStatus.HasValue)
        {
            await socket.CloseAsync(socket.CloseStatus.Value, socket.CloseStatusDescription, CancellationToken.None);
            socket.Dispose();
        }
    }
}