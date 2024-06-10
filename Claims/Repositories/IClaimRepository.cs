using Claims.Controllers;
using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories;

public interface IClaimRepository
{
    Task<IEnumerable<Claim>> GetClaimsAsync(Page page);
    Task<Claim?> GetClaimAsync(string id);
    void Add(Claim claim);
    Task<bool> DeleteItemAsync(string id);
    Task<int> SaveChangesAsync();
}