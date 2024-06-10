using Claims.CustomErrors;
using Claims.Data.ClaimData.Entities;

namespace Claims.Services;

public class ComputePremiumService : IComputePremiumService
{
    private const int BaseDayRate = 1250;
    
    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        ThrowIfInvalidDates(startDate, endDate);

        var premiumPerDayFirst30Days = GetPremiumPerDay(coverType);
        var premiumPerDayTheFollowing150Days = GetPremiumPerDayNext150(coverType, premiumPerDayFirst30Days);
        var premiumPerDayRestOfThePeriod = GetPremiumPerDayRestOfYear(coverType, premiumPerDayTheFollowing150Days);
        
        var insuranceLength = (endDate - startDate).TotalDays; // Validated in ThrowIfInvalidDates
        
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            switch (i)
            {
                case < 30:
                    totalPremium += premiumPerDayFirst30Days;
                    continue;
                case < 180:
                    totalPremium += premiumPerDayTheFollowing150Days;
                    continue;
                case < 365:
                    totalPremium += premiumPerDayRestOfThePeriod;
                    continue;
            }
        }

        return totalPremium;
    }

    private static decimal GetPremiumPerDay(CoverType coverType)
    {
        var multiplier = coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
        };
        return BaseDayRate * multiplier;
    }

    private static decimal GetPremiumPerDayNext150(CoverType coverType, decimal previousPremiumPerDay)
    {
        if (coverType == CoverType.Yacht)
            return previousPremiumPerDay * 0.95m;

        return previousPremiumPerDay * 0.98m;
    }
    
    private static decimal GetPremiumPerDayRestOfYear(CoverType coverType, decimal previousPremiumPerDay)
    {
        if (coverType == CoverType.Yacht)
            return previousPremiumPerDay * 0.97m;

        return previousPremiumPerDay * 0.99m;
    }
    
    public void ThrowIfInvalidDates(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new InvalidDateError("Invalid dates");
        
        if (startDate < DateTime.UtcNow)
            throw new InvalidDateError("StartDate cannot be in the past");

        var duration = (endDate - startDate).TotalDays;
        if (duration > 365)
            throw new InvalidDateError("Total insurance period cannot exceed 1 year");
    }
}