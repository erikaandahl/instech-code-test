using Claims.Data.AuditData.Entities;
using Claims.Messaging.Models;
using Claims.Repositories;

namespace Claims.Auditing;

public class AuditHandler(IServiceScopeFactory serviceScopeFactory) : IAuditHandler
{
    public async Task HandleCommandAsync(AuditLogCommand command)
    {
        switch (command.AuditType)
        {
            case AuditType.Claim:
                await AddClaimAuditAsync(command);
                break;
            case AuditType.Cover:
                await AddCoverAuditAsync(command);
                break;
            default: 
                return;
        }
    }

    private async Task AddClaimAuditAsync(AuditLogCommand command)
    {
        var claimAudit = new ClaimAudit()
        {
            Created = DateTime.UtcNow,
            HttpRequestType = GetRequestType(command),
            ClaimId = command.Id
        };
        
        using var scope = serviceScopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IClaimAuditRepository>();
        repo.Add(claimAudit);
        await repo.SaveChangesAsync();
    }
        
    private async Task AddCoverAuditAsync(AuditLogCommand command)
    {
        var coverAudit = new CoverAudit()
        {
            Created = DateTime.UtcNow,
            HttpRequestType = GetRequestType(command),
            CoverId = command.Id
        };

        using var scope = serviceScopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICoverAuditRepository>();
        repo.Add(coverAudit);
        await repo.SaveChangesAsync();
    }

    private string GetRequestType(AuditLogCommand command)
    {
        return command.CommandType switch
        {
            CommandType.Insert => "POST",
            CommandType.Delete => "DELETE",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}