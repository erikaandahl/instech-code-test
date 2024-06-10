using Claims.Data.AuditData;
using Claims.Data.AuditData.Entities;

namespace Claims.Repositories;

public class ClaimAuditRepository(AuditContext _ctx) : IClaimAuditRepository
{
    public void Add(ClaimAudit claimAudit)
    {
        _ctx.ClaimAudits.Add(claimAudit);
    }
    
    public Task<int> SaveChangesAsync()
    {
        return _ctx.SaveChangesAsync();
    }
}