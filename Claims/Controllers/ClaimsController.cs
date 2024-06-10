using Claims.Repositories.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;


namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(ILogger<ClaimsController> _logger, 
        IClaimService _claimService) : ControllerBase
    {
 
        /// <summary>
        /// Returns a list of Claims
        /// </summary>
        /// <param name="page">Min 1</param>
        /// <param name="pageSize">Max 50</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ClaimDto>> GetAsync(int page = 1, int pageSize = 20)
        {
            return await _claimService.GetClaimsAsync(new Page(page, pageSize));
        }

        /// <summary>
        /// Creates a single Claim
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ClaimDto> CreateAsync(CreateClaimDto claim)
        {
            return await _claimService.AddItemAsync(claim);
        }

        /// <summary>
        /// Deletes a single Claim, by Id
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _claimService.DeleteAsync(id);
        }

        /// <summary>
        /// Gets a single Claim, by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ClaimDto> GetAsync(string id)
        {
            return await _claimService.GetClaimAsync(id);
        }
    }
}
