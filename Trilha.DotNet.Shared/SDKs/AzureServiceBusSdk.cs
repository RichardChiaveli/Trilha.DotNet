namespace Trilha.DotNet.Shared.SDKs;

public class AzureServiceBusSdk(ServiceBusClient client)
{
    protected async Task SendAsync<T>(string topic, T data, Dictionary<string, object> properties, bool compress = true) where T : notnull
    {
        var sender = client.CreateSender(topic);

        try
        {
            var compressFlag = compress;
            string message = compressFlag ? data.Compress() : data.Stringify();
            var bytes = Encoding.UTF8.GetBytes(message);

            var busMessage = new ServiceBusMessage(bytes);

            foreach (var (key, value) in properties)
            {
                busMessage.ApplicationProperties.Add(key, value);
            }

            busMessage.SessionId = Guid.NewGuid().ToString();
            busMessage.MessageId = Guid.NewGuid().ToString();

            var messageBatch = await sender.CreateMessageBatchAsync();

            bool tryAddMessage;

            do
            {
                tryAddMessage = messageBatch.TryAddMessage(busMessage);

                switch (tryAddMessage)
                {
                    case false when !compressFlag:
                        throw new ArgumentException("Message too large to fit in batch.");

                    case false when compressFlag:
                        compressFlag = false;
                        message = data.Stringify();
                        bytes = Encoding.UTF8.GetBytes(message);
                        busMessage = new ServiceBusMessage(bytes);
                        break;
                }
            } while (!tryAddMessage);

            await sender.SendMessagesAsync(messageBatch);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send message to Azure Service Bus.", ex);
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }

    protected async Task ReceiveAsync(string topic, Func<ProcessMessageEventArgs, Task> messageHandler, Func<ProcessErrorEventArgs, Task> errorHandler)
    {
        var processor = client.CreateProcessor(topic);

        try
        {
            processor.ProcessMessageAsync += messageHandler;
            processor.ProcessErrorAsync += errorHandler;

            await processor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to start receiving messages.", ex);
        }
    }
}