using System.ComponentModel.DataAnnotations;
using Claims.Data.ClaimData.Entities;

namespace Claims.Repositories.Models;

public class CreateClaimDto
{
    public string CoverId { get; set; }

    public DateTime Created { get; set; }

    public string Name { get; set; }

    public ClaimType Type { get; set; }

    [Range(typeof(decimal), "0", "100000", ErrorMessage = "Damage cost can't exceed 100000")]
    public decimal DamageCost { get; set; }
}