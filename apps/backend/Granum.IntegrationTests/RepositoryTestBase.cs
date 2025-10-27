using FluentMigrator.Runner;
using Granum.Api.Features.User;
using Granum.Api.Infrastructure;
using Granum.Api.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Granum.IntegrationTests
{
    [Category("Integration")]
    public abstract class RepositoryTestBase
    {
    private static PostgreSqlContainer? _postgresContainer;
    private static IServiceProvider? _serviceProvider;
    protected IAppDbContext DbContext { get; set; } = null!;
    private IServiceScope? _testScope;

        [OneTimeSetUp]
        public static async Task OneTimeSetUpAsync()
        {
            // Start PostgreSQL container once for all tests in this class
            _postgresContainer = new PostgreSqlBuilder()
                .WithDatabase("granumdb")
                .WithUsername("postgres")
                .WithPassword("TestPassword123!")
                .Build();

            await _postgresContainer.StartAsync();

            // Create service provider with container connection string
            var services = new ServiceCollection();
            var connectionString = _postgresContainer.GetConnectionString();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
                    .ConfigureWarnings(w =>
                        w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            _serviceProvider = services.BuildServiceProvider();

            // Apply FluentMigrator migrations
            var migrationRunner = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(cfg => cfg
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(InitialCreate).Assembly).For.Migrations())
                .BuildServiceProvider()
                .GetRequiredService<IMigrationRunner>();

            migrationRunner.MigrateUp();
        }

        [SetUp]
        public virtual async Task SetUpAsync()
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("OneTimeSetUp was not called");

            // Create a scope that will live for the entire test
            _testScope = _serviceProvider.CreateScope();
            DbContext = _testScope.ServiceProvider.GetRequiredService<IAppDbContext>();

            // Clear all data by deleting from tables (respecting foreign keys)
            await ClearDatabaseAsync(DbContext);
        }

        [TearDown]
        public virtual async Task TearDownAsync()
        {
            if (_testScope != null && DbContext != null)
            {
                // Delete all data to reset for next test
                await ClearDatabaseAsync(DbContext);
            }

            // Dispose the scope which will dispose the context
            _testScope?.Dispose();
            _testScope = null;
            DbContext = null!;
        }

        [OneTimeTearDown]
        public static async Task OneTimeTearDownAsync()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (_postgresContainer != null)
            {
                await _postgresContainer.StopAsync();
                await _postgresContainer.DisposeAsync();
            }
        }

        private static async Task ClearDatabaseAsync(IAppDbContext dbContext)
        {
            // Clear tables in order respecting foreign key constraints
            // Delete child tables first, then parent tables
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"LocationFeatures\"");
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"ServiceLocations\"");
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Users\"");
        }

    }
}
