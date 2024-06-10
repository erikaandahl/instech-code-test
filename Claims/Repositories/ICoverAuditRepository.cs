using Claims.Data.AuditData.Entities;

namespace Claims.Repositories;

public interface ICoverAuditRepository
{
    void Add(CoverAudit coverAudit);
    Task<int> SaveChangesAsync();
}