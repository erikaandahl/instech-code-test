using Claims.Data.ClaimData.Entities;

namespace Claims.Services;

public interface IComputePremiumService
{
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    void ThrowIfInvalidDates(DateTime startDate, DateTime endDate);
}