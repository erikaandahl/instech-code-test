using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories.Models;

public class CreateCoverDto
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CoverType Type { get; set; }
    
}