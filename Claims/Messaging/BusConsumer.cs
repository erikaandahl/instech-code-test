using Azure.Messaging.ServiceBus;
using Claims.Auditing;
using Claims.Messaging.Models;

namespace Claims.Messaging;

public class BusConsumer(IAuditHandler _auditHandler, ServiceBusSettings _settings, 
    ILogger<BusConsumer> _logger) : IBusConsumer
{
    private readonly ServiceBusClient _client = new(_settings.ConnectionString);
    private ServiceBusProcessor? _processor;

    public async Task RegisterOnMessageHandlerAndReceiveMessages()
    {
        var serviceBusProcessorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false,
        };

        _processor = _client.CreateProcessor(_settings.QueueName, serviceBusProcessorOptions);
        _processor.ProcessMessageAsync += ProcessMessagesAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
        await _processor.StartProcessingAsync();
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Message handler encountered an exception");
        
        return Task.CompletedTask;
    }

    private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
    {
        var auditLog = args.Message.Body.ToObjectFromJson<AuditLogCommand>();
        await _auditHandler.HandleCommandAsync(auditLog);
        await args.CompleteMessageAsync(args.Message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor != null)
        {
            await _processor.DisposeAsync();
        }

        await _client.DisposeAsync();
    }

    public async Task CloseQueueAsync()
    {
        if (_processor != null)
            await _processor.CloseAsync();
    }
}