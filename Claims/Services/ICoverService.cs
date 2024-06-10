using Claims.Controllers;
using Claims.Repositories.Models;

namespace Claims.Services;

public interface ICoverService
{
    Task<IEnumerable<CoverDto>> GetCoversAsync(Page page);
    Task<CoverDto> GetCoverAsync(string id);
    Task<CoverDto> AddCoverAsync(CreateCoverDto input);
    Task DeleteAsync(string id);
}