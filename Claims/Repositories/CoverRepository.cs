using Claims.Controllers;
using Claims.Data.ClaimData;
using Claims.Data.ClaimData.Entities;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories;

public class CoverRepository(ClaimsContext _ctx) : ICoverRepository
{
    public async Task<IEnumerable<Cover>> GetCoversAsync(Page page)
    {
        return await _ctx.Covers
            .Skip(page.Skip)
            .Take(page.PageSize)
            .ToArrayAsync();
    }

    public Task<Cover?> GetAsync(string id)
    {
        return _ctx.Covers
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public void Add(Cover cover)
    {
        _ctx.Covers.Add(cover);
    }
    
    public Task<int> SaveChangesAsync()
    {
        return _ctx.SaveChangesAsync();
    }

    public async Task<bool> DeleteItemAsync(string id)
    {
        var cover = await _ctx.FindAsync<Cover>(id);
        if (cover is null) return false;
        _ctx.Covers.Remove(cover);
        return true;
    }
}