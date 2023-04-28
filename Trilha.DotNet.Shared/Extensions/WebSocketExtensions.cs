namespace Trilha.DotNet.Shared.Extensions;

public static class WebSocketExtensions
{
    public static async Task SendObjectAsync<T>(this WebSocket socket, T payload)
    {
        var bytes = Encoding.UTF8.GetBytes(payload!.Stringify());

        await socket.SendAsync(
            new ArraySegment<byte>(bytes)
            , WebSocketMessageType.Text
            , true
            , CancellationToken.None);
    }

    public static async Task<T> ReceiveObjectAsync<T>(this WebSocket socket)
    {
        var buffer = new byte[1024];

        var response = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer)
            , CancellationToken.None);

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