using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Granum.Api.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Granum.IntegrationTests.Infrastructure
{
    [Category("Integration")]
    public abstract class IntegrationTestBase
    {
        protected WebApplicationFactory<Program> Factory { get; private set; } = null!;
        protected HttpClient Client { get; private set; } = null!;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        [SetUp]
        public virtual void SetUp()
        {
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        RemoveDbContext<AppDbContext>(services);

                        services.AddDbContext<AppDbContext>(options =>
                            options.UseInMemoryDatabase($"IntegrationTest_{Guid.NewGuid()}"));
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
            where TContext : class
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TContext));
            if (descriptor != null)
                services.Remove(descriptor);
        }
    }
}
