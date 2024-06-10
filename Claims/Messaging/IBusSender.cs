using Claims.Messaging.Models;

namespace Claims.Messaging;

public interface IBusSender
{
    Task SendMessageAsync(AuditLogCommand command);
}