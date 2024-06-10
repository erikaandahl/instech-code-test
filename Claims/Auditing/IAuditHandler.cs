using Claims.Messaging.Models;

namespace Claims.Auditing;

public interface IAuditHandler
{
    Task HandleCommandAsync(AuditLogCommand command);
}