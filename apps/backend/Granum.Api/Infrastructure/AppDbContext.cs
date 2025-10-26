using Granum.Api.Features.User;
using Granum.Api.Features.ServiceLocation;
using Microsoft.EntityFrameworkCore;

namespace Granum.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ServiceLocation> ServiceLocations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Fallback for migrations and design-time scenarios
        if (optionsBuilder.IsConfigured) return;
        
        optionsBuilder.UseSqlServer("Server=(local);Database=GranumDb;Trusted_Connection=true;");
        
        // Suppress pending model changes warning for testing scenarios
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Table Per Hierarchy (TPH) - map Customer and Contractor to Users table
        modelBuilder.Entity<User>()
            .ToTable("Users")
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Customer>("Customer")
            .HasValue<Contractor>("Contractor");

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
