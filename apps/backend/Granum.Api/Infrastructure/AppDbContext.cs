using Granum.Api.Features.Estimate;
using Granum.Api.Features.Location;
using Granum.Api.Features.Service;
using Granum.Api.Features.User;
using Microsoft.EntityFrameworkCore;

namespace Granum.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) // TODO use interface and DbContext from EF Core
{
    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Estimate> Estimates { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<Frequency> Frequency { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Only seed for in-memory database
        if (!Database.IsInMemory()) return;

        modelBuilder.Entity<Contractor>().HasData(
            new Contractor
            {
                Id = 1,
                Name = "John's Landscaping",
                // Add other required Contractor properties here
            }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 2,
                Name = "Jane Smith",
                // Add other required Customer properties here
            }
        );
    }
}