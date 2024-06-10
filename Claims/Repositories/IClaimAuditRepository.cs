using Claims.Data.AuditData.Entities;

namespace Claims.Repositories;

public interface IClaimAuditRepository
{
    void Add(ClaimAudit claimAudit);
    Task<int> SaveChangesAsync();
}