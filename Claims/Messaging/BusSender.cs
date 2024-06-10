using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Claims.Messaging.Models;

namespace Claims.Messaging;

public class BusSender : IBusSender
{ 
    private readonly ServiceBusSender _clientSender;

    public BusSender(ServiceBusSettings settings)
    {
        var client = new ServiceBusClient(settings.ConnectionString);
        _clientSender = client.CreateSender(settings.QueueName);
    }
    
    public async Task SendMessageAsync(AuditLogCommand command)
    {
        var messagePayload = JsonSerializer.Serialize(command);
        var message = new ServiceBusMessage(messagePayload);
        await _clientSender.SendMessageAsync(message);
    }
}