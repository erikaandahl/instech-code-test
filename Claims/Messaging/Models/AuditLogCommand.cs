namespace Claims.Messaging.Models;

public class AuditLogCommand
{
    public CommandType CommandType { get; set; }
    public AuditType AuditType { get; set; }
    public string Id { get; set; }
}

public enum CommandType
{
    Insert,
    Delete
}

public enum AuditType
{
    Claim,
    Cover
}