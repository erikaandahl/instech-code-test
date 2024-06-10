using Claims.Controllers;
using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories;

public interface ICoverRepository
{
    Task<IEnumerable<Cover>> GetCoversAsync(Page page);
    Task<Cover?> GetAsync(string id);
    void Add(Cover cover);
    Task<int> SaveChangesAsync();
    Task<bool> DeleteItemAsync(string id);
}