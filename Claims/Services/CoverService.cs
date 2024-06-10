using Claims.Controllers;
using Claims.CustomErrors;
using Claims.Data.ClaimData.Entities;
using Claims.Messaging;
using Claims.Messaging.Models;
using Claims.Repositories;
using Claims.Repositories.Models;

namespace Claims.Services;

public class CoverService(ICoverRepository _repo, IBusSender _bus, IComputePremiumService _premiumService) : ICoverService
{
    public async Task<IEnumerable<CoverDto>> GetCoversAsync(Page page)
    {
        page.VerifyPageSize(50);
        var entities = await _repo.GetCoversAsync(page);
        return entities.Select(x => x.MapToCoverDto());
    }
    
    public async Task<CoverDto> GetCoverAsync(string id)
    {
        var item = await _repo.GetAsync(id);
        if (item is null) throw new NotFoundError("Cover not found");
        return item.MapToCoverDto();
    }

    public async Task<CoverDto> AddCoverAsync(CreateCoverDto input)
    {
        _premiumService.ThrowIfInvalidDates(input.StartDate, input.EndDate);
        var cover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Type = input.Type,
            Premium = _premiumService.ComputePremium(input.StartDate, input.EndDate, input.Type)
        };
        _repo.Add(cover);
        await _repo.SaveChangesAsync();

        await _bus.SendMessageAsync(new AuditLogCommand
        {
            Id = cover.Id,
            AuditType = AuditType.Cover,
            CommandType = CommandType.Insert
        });
        return cover.MapToCoverDto();
    }
    
    public async Task DeleteAsync(string id)
    {
        var removed = await _repo.DeleteItemAsync(id);

        if (removed)
        {
            await _repo.SaveChangesAsync();

            await _bus.SendMessageAsync(new AuditLogCommand
            {
                Id = id,
                AuditType = AuditType.Claim,
                CommandType = CommandType.Delete
            });
        }
    }
}
