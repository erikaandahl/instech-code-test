using Claims.CustomErrors;
using Claims.Data.ClaimData.Entities;
using Claims.Repositories.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(ILogger<CoversController> _logger, 
    ICoverService _coverService, IComputePremiumService _premiumService) : ControllerBase
{
    /// <summary>
    /// Calculates the premium for Cover
    /// </summary>
    /// <param name="startDate">ISO 8601</param>
    /// <param name="endDate">ISO 8601. Has to be later than startDate</param>
    /// <param name="coverType"></param>
    /// <returns></returns>
    [HttpPost("compute")]
    public ActionResult ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        // All error handling should be done in one place...
        // Not as try catch in a controller, but rather using IExceptionFilter
        try
        {
            var premium = _premiumService.ComputePremium(startDate, endDate, coverType);
            return Ok(premium);
        }
        catch (BadRequestError bre)
        {
            _logger.LogInformation(bre.Message);
            return BadRequest(bre.Message);
        }
    }

    /// <summary>
    /// Gets a list of Covers
    /// </summary>
    /// <param name="page">Min 1</param>
    /// <param name="pageSize">Max 50</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IEnumerable<CoverDto>> GetAsync(int page = 1, int pageSize = 20)
    {
        return await _coverService.GetCoversAsync(new Page(page, pageSize));
    }

    /// <summary>
    /// Returns a single Cover, by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> GetAsync(string id)
    {
        try
        {
            var result = await _coverService.GetCoverAsync(id);
            return Ok(result);
        }
        catch (NotFoundError nfr)
        {
            _logger.LogInformation(nfr.Message);
            return NotFound(nfr.Message);
        }
    }

    /// <summary>
    /// Creates a single Cover
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<ClaimDto>> CreateAsync(CreateCoverDto input)
    {
        try
        {
            var cover = await _coverService.AddCoverAsync(input);
            return Ok(cover);
        }
        catch (BadRequestError bre)
        {
            _logger.LogInformation(bre.Message);
            return BadRequest(bre.Message);
        }
    }

    /// <summary>
    /// Deletes a single Cover, by Id
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coverService.DeleteAsync(id);
    }


}
