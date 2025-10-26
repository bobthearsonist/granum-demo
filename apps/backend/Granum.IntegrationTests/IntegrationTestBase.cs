using System.Text;
using System.Text.Json;
using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Program = Granum.Api.Program;

namespace Granum.IntegrationTests
{
    [Category("Integration")]
    public abstract class IntegrationTestBase
    {
        private WebApplicationFactory<Program> Factory { get; set; } = null!;
        private HttpClient Client { get; set; } = null!;
        private string DatabaseName { get; set; } = null!;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [SetUp]
        public virtual void SetUp()
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

        [TearDown]
        public virtual void TearDown()
        {
            Client.Dispose();
            Factory.Dispose();
        }

        protected async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await Client.PostAsync(url, httpContent);
        }

        protected async Task<HttpResponseMessage> PutJsonAsync<T>(string url, T content)
        {
            var json = JsonSerializer.Serialize(content, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await Client.PutAsync(url, httpContent);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
            => await Client.GetAsync(url);

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
            => await Client.DeleteAsync(url);

        protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
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
