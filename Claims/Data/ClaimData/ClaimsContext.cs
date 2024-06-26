using Claims.Controllers;
using Claims.Data.ClaimData.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data.ClaimData;

public class ClaimsContext : DbContext
{
    public DbSet<Claim> Claims { get; init; }
    public DbSet<Cover>  Covers { get; init; }

    public ClaimsContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Claim>().ToCollection("claims");
        modelBuilder.Entity<Cover>().ToCollection("covers");
    }
}