using Claims.Data.AuditData;
using Claims.Data.AuditData.Entities;

namespace Claims.Repositories;

public class CoverAuditRepository(AuditContext _ctx) : ICoverAuditRepository
{
    public void Add(CoverAudit coverAudit)
    {
        _ctx.CoverAudits.Add(coverAudit);
    }
    
    public Task<int> SaveChangesAsync()
    {
        return _ctx.SaveChangesAsync();
    }
}