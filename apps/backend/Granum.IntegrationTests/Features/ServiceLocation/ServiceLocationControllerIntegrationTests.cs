using System.Net;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class ServiceLocationControllerIntegrationTests : IntegrationTestBase
    {
        private const string ServiceLocationsUrl = "/api/service-locations";
        private const int TestCustomerId = 1;

        [Test]
        public async Task CreateServiceLocation_WithValidData_ReturnsCreated()
        {
            var createRequest = new
            {
                customerId = TestCustomerId,
                name = "Main Office",
                address = "123 Main St"
            };

            var response = await PostJsonAsync(ServiceLocationsUrl, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateServiceLocation_WithoutName_ReturnsBadRequest()
        {
            var invalidRequest = new
            {
                customerId = TestCustomerId,
                name = (string?)null,
                address = "123 Main St"
            };

            var response = await PostJsonAsync(ServiceLocationsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateServiceLocation_WithoutAddress_ReturnsBadRequest()
        {
            var invalidRequest = new
            {
                customerId = TestCustomerId,
                name = "Main Office",
                address = (string?)null
            };

            var response = await PostJsonAsync(ServiceLocationsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateServiceLocation_WithInvalidProperty_ReturnsBadRequest()
        {
            var invalidRequest = new
            {
                customerId = TestCustomerId,
                name = "Main Office",
                address = "123 Main St",
                invalidProperty = "should not be here"
            };

            var response = await PostJsonAsync(ServiceLocationsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllServiceLocations_ReturnsOkWithList()
        {
            var response = await GetAsync(ServiceLocationsUrl);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllServiceLocations_AfterCreating_ReturnsListWithLocation()
        {
            var createRequest = new
            {
                customerId = TestCustomerId,
                name = "Test Location",
                address = "456 Test Ave"
            };
            await PostJsonAsync(ServiceLocationsUrl, createRequest);

            var response = await GetAsync(ServiceLocationsUrl);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task GetServiceLocation_ByValidId_ReturnsOkWithLocation()
        {
            // Arrange - Create a location first
            var createRequest = new
            {
                customerId = TestCustomerId,
                name = "Get Test Location",
                address = "789 Get Ave"
            };
            var createResponse = await PostJsonAsync(ServiceLocationsUrl, createRequest);
            var createdLocation = await DeserializeResponseAsync<JObject>(createResponse);
            var locationId = createdLocation?["id"]?.Value<int>();

            // Act
            var getResponse = await GetAsync($"{ServiceLocationsUrl}/{locationId}");
            var retrievedLocation = await DeserializeResponseAsync<JObject>(getResponse);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            retrievedLocation.Should().NotBeNull();
            retrievedLocation?["name"]?.Value<string>().Should().Be("Get Test Location");
        }

        [Test]
        public async Task DeleteServiceLocation_WithValidId_ReturnsNoContent()
        {
            // Arrange - Create a location first
            var createRequest = new
            {
                customerId = TestCustomerId,
                name = "Delete Test Location",
                address = "101 Delete Ave"
            };
            var createResponse = await PostJsonAsync(ServiceLocationsUrl, createRequest);
            var createdLocation = await DeserializeResponseAsync<JObject>(createResponse);
            var locationId = createdLocation?["id"]?.Value<int>();

            // Act
            var deleteResponse = await DeleteAsync($"{ServiceLocationsUrl}/{locationId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify it's deleted
            var getResponse = await GetAsync($"{ServiceLocationsUrl}/{locationId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
