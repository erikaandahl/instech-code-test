using Claims.Controllers;
using Claims.Data.ClaimData;
using Claims.Data.ClaimData.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories;

public class ClaimRepository(ClaimsContext _ctx) : IClaimRepository
{
    public async Task<IEnumerable<Claim>> GetClaimsAsync(Page page)
    {
        return await _ctx.Claims
            .Skip(page.Skip)
            .Take(page.PageSize)
            .ToListAsync();
    }

    public async Task<Claim?> GetClaimAsync(string id)
    {
        return await _ctx.Claims
            .Where(claim => claim.Id == id)
            .FirstOrDefaultAsync();
    }

    public void Add(Claim claim)
    {
        _ctx.Claims.Add(claim);
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        var claim = await _ctx.FindAsync<Claim>(id);
        if (claim is null) return false;
        _ctx.Claims.Remove(claim);
        return true;
    }

    public Task<int> SaveChangesAsync()
    {
        return _ctx.SaveChangesAsync();
    }
}