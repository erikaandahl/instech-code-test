using Claims.Data.ClaimData.Entities;
using Claims.Services;
using Xunit;

namespace Claims.Tests;

public class ComputePremiumServiceTests
{
    [Theory]
    [InlineData(1, CoverType.ContainerShip, "1625.0")]  // 1250 * 1.3 * 1
    [InlineData(1, CoverType.Yacht, "1375.0")]          // 1250 * 1.1 * 1
    [InlineData(1, CoverType.BulkCarrier, "1625.0")]    // 1250 * 1.3 * 1
    [InlineData(1, CoverType.PassengerShip, "1500.0")]  // 1250 * 1.2 * 1
    [InlineData(45, CoverType.ContainerShip, "72637.5")]  // (1250 * 1.3 * 30) + (1250 * 1.3 * 0.98 * 15)
    [InlineData(45, CoverType.Yacht, "60843.75")]          // (1250 * 1.1 * 30) + (1250 * 1.1 * 0.95 * 15)
    [InlineData(45, CoverType.BulkCarrier, "72637.5")]    // (1250 * 1.3 * 30) + (1250 * 1.3 * 0.98 * 15)
    [InlineData(45, CoverType.PassengerShip, "67050")]  // (1250 * 1.2 * 30) + (1250 * 1.2 * 0.98 * 15)
    [InlineData(300, CoverType.ContainerShip, "476814")]  // (1250 * 1.3 * 30) + (1250 * 1.3 * 0.98 * 150) + (1250 * 1.3 * 0.98 * 0.99 * 120)
    [InlineData(250, CoverType.Yacht, "325881.875")]          // (1250 * 1.1 * 30) + (1250 * 1.1 * 0.95 * 150) + (1250 * 1.1 * 0.95 * 0.97 * 70)
    [InlineData(200, CoverType.BulkCarrier, "319156.5")]    // (1250 * 1.3 * 30) + (1250 * 1.3 * 0.98 * 150) + (1250 * 1.3 * 0.98 * 0.99 * 20)
    [InlineData(350, CoverType.PassengerShip, "512901")]  // (1250 * 1.2 * 30) + (1250 * 1.2 * 0.98 * 150) + (1250 * 1.2 * 0.98 * 0.99 * 170)
    public void Compute_Premium(int days, CoverType coverType, string expectedParameter)
    {
        var expected = Convert.ToDecimal(expectedParameter);
        
        var start = DateTime.UtcNow.AddHours(1);
        var end = start.AddDays(days);
        var computeService = new ComputePremiumService();

        var result = computeService.ComputePremium(start, end, coverType);
        
        Assert.Equal(expected, result);
    }
}