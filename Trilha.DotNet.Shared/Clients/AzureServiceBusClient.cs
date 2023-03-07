namespace Trilha.DotNet.Shared.Clients;

public sealed class AzureServiceBusClient
{
    private readonly ServiceBusClient _client;

    public AzureServiceBusClient(string endpoint, string sharedAccessKeyName, string sharedAccessKey)
    {
        _client = new ServiceBusClient(endpoint, new AzureNamedKeyCredential(sharedAccessKeyName, sharedAccessKey));
    }

    public AzureServiceBusClient(string connectionString)
    {
        _client = new ServiceBusClient(connectionString);
    }

    public async Task SendAsync<T>(string topic, T data, Dictionary<string, object> properties, bool compress = true) where T : notnull
    {
        while (true)
        {
            var message = compress ? data.Compress() : data.Stringify();

            var bytes = Encoding.UTF8.GetBytes(message);
            var busMessage = new ServiceBusMessage(bytes);

            foreach (var (key, value) in properties)
            {
                busMessage.ApplicationProperties.Add(key, value);
            }

            busMessage.SessionId = Guid.NewGuid().ToString();
            busMessage.MessageId = Guid.NewGuid().ToString();

            var sender = _client.CreateSender(topic);
            var messageBatch = await sender.CreateMessageBatchAsync();

            var tryAddMessage = messageBatch.TryAddMessage(busMessage);

            if (!tryAddMessage)
            {
                await sender.DisposeAsync();
                await _client.DisposeAsync();

                if (compress)
                    throw new ArgumentException("Message too large to fit in batch!");

                compress = true;
                continue;
            }

            await sender.SendMessagesAsync(messageBatch);
            break;
        }
    }

    public async Task ReceiveAsync(string topic, Func<ProcessMessageEventArgs, Task> messageHandler, Func<ProcessErrorEventArgs, Task> errorHandler)
    {
        var processor = _client.CreateProcessor(topic);
        processor.ProcessMessageAsync += messageHandler;
        processor.ProcessErrorAsync += errorHandler;

        await processor.StartProcessingAsync();
    }
}