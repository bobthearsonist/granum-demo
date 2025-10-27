using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Refit;
using Program = Granum.Api.Program;

namespace Granum.IntegrationTests
{
    [Category("Integration")]
    public abstract class IntegrationTestBase
    {
        private WebApplicationFactory<Program> Factory { get; set; } = null!;
        protected HttpClient Client { get; set; } = null!;
        private string DatabaseName { get; set; } = null!;

        protected static readonly JsonSerializerSettings JsonSettings = MvcBuilderExtensions.GetJsonSerializerSettings();

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            DatabaseName = $"IntegrationTest_{Guid.NewGuid()}";

            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        RemoveDbContext<AppDbContext>(services);

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(DatabaseName);
                            options.EnableSensitiveDataLogging();
                        });
                    });
                });

            Client = Factory.CreateClient();
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            Client.Dispose();
            Factory.Dispose();
        }

        protected T CreateApi<T>() where T : class
        {
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(JsonSettings)
            };
            return RestService.For<T>(Client, refitSettings);
        }

        private static void RemoveDbContext<TContext>(IServiceCollection services)
            where TContext : DbContext
        {
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<TContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(TContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
        }
    }
}
