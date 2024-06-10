using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories.Models;

public class ClaimDto
{
    public string Id { get; set; }
    public string CoverId { get; set; }

    public DateTime Created { get; set; }

    public string Name { get; set; }

    public ClaimType Type { get; set; }

    public decimal DamageCost { get; set; }
}