using System.Net;
using System.Text;
using Claims.Data.ClaimData.Entities;
using Claims.Repositories.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {
        [Fact]
        public async Task Get_Claims()
        {
            var client = CreateHttpClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var claims = JsonConvert.DeserializeObject<IEnumerable<ClaimDto>>(result);
            
            Assert.NotNull(claims);
            Assert.True(claims.Count() <= 20); 
        }
        
        [Fact]
        public async Task Create_Claim()
        {
            var client = CreateHttpClient();
            var validCoverId = await GetValidCoverId(client);
            var claimToAdd = new CreateClaimDto
            {
                CoverId = validCoverId,
                Created = GetClaimCreatedDate(),
                DamageCost = 15236.45m,
                Type = ClaimType.BadWeather,
                Name = "The best claim ever"
            };
            

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(claimToAdd),
                Encoding.UTF8,
                "application/json");
            
            var response = await client.PostAsync("/Claims", jsonContent);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var claim = JsonConvert.DeserializeObject<ClaimDto>(result);
            
            Assert.NotNull(claim);
            Assert.NotNull(claim.Id);
            Assert.True(claim.Name == claimToAdd.Name); 
            Assert.True(claim.CoverId == claimToAdd.CoverId); 
        }

        [Fact]
        public async Task Create_Claim_Then_Get_By_Id()
        {
            var client = CreateHttpClient();
            var validCoverId = await GetValidCoverId(client);
            
            var claimToAdd = new CreateClaimDto
            {
                CoverId = validCoverId,
                Created = GetClaimCreatedDate(),
                DamageCost = 36.5m,
                Type = ClaimType.Fire,
                Name = "The second best claim ever"
            };

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(claimToAdd),
                Encoding.UTF8,
                "application/json");
            
            var postResponse = await client.PostAsync("/Claims", jsonContent);
            var postResult = await postResponse.Content.ReadAsStringAsync();
            var claim1 = JsonConvert.DeserializeObject<ClaimDto>(postResult);
            
            var getResponse = await client.GetAsync($"/Claims/{claim1!.Id}");

            var result = await getResponse.Content.ReadAsStringAsync();
            var claim2 = JsonConvert.DeserializeObject<ClaimDto>(result);
            
            Assert.NotNull(claim2);
            Assert.True(claim2.Name == claimToAdd.Name); 
            Assert.True(claim2.CoverId == claimToAdd.CoverId);
            Assert.True(claim2.Type == ClaimType.Fire); 
        }
        
        [Fact]
        public async Task Create_Claim_With_Too_High_DamageCost()
        {
            var client = CreateHttpClient();
            var validCoverId = await GetValidCoverId(client);
            
            var claimToAdd = new CreateClaimDto
            {
                CoverId = validCoverId,
                Created = GetClaimCreatedDate(),
                DamageCost = 100001m,
                Type = ClaimType.Fire,
                Name = "The one with too high cost"
            };
            
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(claimToAdd),
                Encoding.UTF8,
                "application/json");
            
            var postResponse = await client.PostAsync("/Claims", jsonContent);
            
            Assert.True(postResponse.StatusCode == HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task Create_Claim_With_Invalid_CoverId()
        {
            var client = CreateHttpClient();
            
            var claimToAdd = new CreateClaimDto
            {
                CoverId = "ShouldBeInvalid",
                Created = GetClaimCreatedDate(),
                DamageCost = 100m,
                Type = ClaimType.Fire,
                Name = "The one with invalid cover ID"
            };
            
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(claimToAdd),
                Encoding.UTF8,
                "application/json");
            
            var postResponse = await client.PostAsync("/Claims", jsonContent);
            
            Assert.True(postResponse.StatusCode == HttpStatusCode.InternalServerError);
        }
        
        [Fact]
        public async Task Create_Claim_With_Invalid_Date()
        {
            var client = CreateHttpClient();
            var validCoverId = await GetValidCoverId(client);
            
            var claimToAdd = new CreateClaimDto
            {
                CoverId = validCoverId,
                Created = DateTime.UtcNow.AddDays(-1),
                DamageCost = 100m,
                Type = ClaimType.Fire,
                Name = "Invalid date claim"
            };
            
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(claimToAdd),
                Encoding.UTF8,
                "application/json");
            
            var postResponse = await client.PostAsync("/Claims", jsonContent);
            
            Assert.True(postResponse.StatusCode == HttpStatusCode.InternalServerError);
        }
        
        private HttpClient CreateHttpClient()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                    {});

            return application.CreateClient();
        }
        
        private async Task<string> GetValidCoverId(HttpClient client)
        {
            var coverToAdd = new CreateCoverDto
            {
                StartDate = DateTime.UtcNow.AddMinutes(1),
                EndDate = DateTime.UtcNow.AddDays(1),
                Type = CoverType.ContainerShip
            };

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(coverToAdd),
                Encoding.UTF8,
                "application/json");
            
            var response = await client.PostAsync("/Covers", jsonContent);

            var result = await response.Content.ReadAsStringAsync();
            var cover = JsonConvert.DeserializeObject<CoverDto>(result);
            return cover!.Id;
        }

        private DateTime GetClaimCreatedDate()
        {
            return DateTime.UtcNow.AddMinutes(2);
        }
    }
}
