using Claims.Repositories.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Data.ClaimData.Entities
{
    public class Claim
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("coverId")]
        public string CoverId { get; set; }

        [BsonElement("created")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Created { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("claimType")]
        public ClaimType Type { get; set; }

        [BsonElement("damageCost")]
        public decimal DamageCost { get; set; }

        public ClaimDto MapToClaimDto()
        {
            return new ClaimDto
            {
                Id = Id,
                CoverId = CoverId,
                Created = Created,
                Name = Name,
                Type = Type,
                DamageCost = DamageCost
            };
        }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
