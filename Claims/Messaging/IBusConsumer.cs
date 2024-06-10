namespace Claims.Messaging;

public interface IBusConsumer
{
    Task RegisterOnMessageHandlerAndReceiveMessages();
    ValueTask DisposeAsync();
    Task CloseQueueAsync();
}