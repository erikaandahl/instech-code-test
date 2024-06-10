using System.Net;
using System.Text;
using Claims.Data.ClaimData.Entities;
using Claims.Repositories.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Claims.Tests;

public class CoversControllerTests
{
    [Fact]
    public async Task Get_Covers()
    {
        var client = CreateHttpClient();

        var response = await client.GetAsync("/Covers");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var covers = JsonConvert.DeserializeObject<IEnumerable<CoverDto>>(result);
            
        Assert.NotNull(covers);
        Assert.True(covers.Count() <= 20); 
    }
    
    [Fact]
    public async Task Create_Cover()
    {
        var client = CreateHttpClient();
        
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
            
        Assert.NotNull(cover);
        Assert.NotNull(cover.Id);
        Assert.True(coverToAdd.Type == cover.Type);
    }
    
    [Fact]
    public async Task Create_Cover_With_StartDate_In_The_Past()
    {
        var client = CreateHttpClient();
        
        var coverToAdd = new CreateCoverDto
        {
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            Type = CoverType.ContainerShip
        };

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(coverToAdd),
            Encoding.UTF8,
            "application/json");
            
        var response = await client.PostAsync("/Covers", jsonContent);
        
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task Create_Cover_With_Reversed_Dates()
    {
        var client = CreateHttpClient();
        
        var coverToAdd = new CreateCoverDto
        {
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(-1),
            Type = CoverType.ContainerShip
        };

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(coverToAdd),
            Encoding.UTF8,
            "application/json");
            
        var response = await client.PostAsync("/Covers", jsonContent);
        
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError);
    }
    
    [Fact]
    public async Task Create_Cover_Exceeds_A_Year()
    {
        var client = CreateHttpClient();
        
        var coverToAdd = new CreateCoverDto
        {
            StartDate = DateTime.UtcNow.AddMinutes(1),
            EndDate = DateTime.UtcNow.AddYears(1).AddDays(1),
            Type = CoverType.BulkCarrier
        };

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(coverToAdd),
            Encoding.UTF8,
            "application/json");
            
        var response = await client.PostAsync("/Covers", jsonContent);
        
        Assert.True(response.StatusCode == HttpStatusCode.InternalServerError);
    }
    
    private HttpClient CreateHttpClient()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ =>
                {});

        return application.CreateClient();
    }
}