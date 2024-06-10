using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories.Models;

public class CoverDto
{
    public string Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CoverType Type { get; set; }

    public decimal Premium { get; set; }
}