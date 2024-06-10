using Claims.Controllers;
using Claims.Repositories.Models;

namespace Claims.Services;

public interface IClaimService
{
    Task<ClaimDto> GetClaimAsync(string id);
    Task<IEnumerable<ClaimDto>> GetClaimsAsync(Page page);
    Task<ClaimDto> AddItemAsync(CreateClaimDto item);
    Task DeleteAsync(string id);
}