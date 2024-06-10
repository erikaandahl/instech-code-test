using Claims.Controllers;
using Claims.CustomErrors;
using Claims.Data.ClaimData.Entities;
using Claims.Messaging;
using Claims.Messaging.Models;
using Claims.Repositories;
using Claims.Repositories.Models;

namespace Claims.Services;

public class ClaimService(IClaimRepository _repo, IBusSender _bus, ICoverRepository _coverRepository) : IClaimService
{
    public async Task<ClaimDto> GetClaimAsync(string id)
    {
        var claim = await _repo.GetClaimAsync(id);
        if (claim is null)
            throw new Exception("Claim not found");
        return claim.MapToClaimDto();
    }

    public async Task<IEnumerable<ClaimDto>> GetClaimsAsync(Page page)
    {
        page.VerifyPageSize(50);
        var entities = await _repo.GetClaimsAsync(page);
        var mapped = entities.Select(x => x.MapToClaimDto());
        return mapped;
    }
    
    public async Task<ClaimDto> AddItemAsync(CreateClaimDto item)
    {
        await ValidateOrThrowClaim(item);
        
        var claim = new Claim
        {
            Id = Guid.NewGuid().ToString(),
            CoverId = item.CoverId,
            Created = item.Created,
            Name = item.Name,
            Type = item.Type,
            DamageCost = item.DamageCost
        };
        _repo.Add(claim);
        await _repo.SaveChangesAsync();

        await _bus.SendMessageAsync(new AuditLogCommand
        {
            Id = claim.Id,
            AuditType = AuditType.Claim,
            CommandType = CommandType.Insert
        });

        return claim.MapToClaimDto();
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
    
    private async Task ValidateOrThrowClaim(CreateClaimDto item)
    {
        var cover = await _coverRepository.GetAsync(item.CoverId);

        if (cover is null)
            throw new NotFoundError("Cover not found");
        if (cover.StartDate > item.Created)
            throw new InvalidDateError("Creation date can not be before cover start date");
        if (cover.EndDate < item.Created)
            throw new InvalidDateError("Creation date can not be after cover end date");
    }
}