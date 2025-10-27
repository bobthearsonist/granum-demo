using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class ServiceLocationControllerIntegrationTests : IntegrationTestBase
    {
        private IServiceLocationApi _api = null!;
        private const int TestCustomerId = 1;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            _api = CreateApi<IServiceLocationApi>();
        }

        [Test]
        public async Task CreateServiceLocation_WithValidData_ReturnsCreated()
        {
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Main Office",
                Address = "123 Main St"
            };

            var response = await _api.CreateAsync(location);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateServiceLocation_WithoutName_ReturnsBadRequest()
        {
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = null!,
                Address = "123 Main St"
            };

            var response = await _api.CreateAsync(location);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateServiceLocation_WithoutAddress_ReturnsBadRequest()
        {
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Main Office",
                Address = null!
            };

            var response = await _api.CreateAsync(location);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllServiceLocations_ReturnsOkWithList()
        {
            var response = await _api.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllServiceLocations_AfterCreating_ReturnsListWithLocation()
        {
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Test Location",
                Address = "456 Test Ave"
            };
            await _api.CreateAsync(location);

            var response = await _api.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task GetServiceLocation_ByValidId_ReturnsOkWithLocation()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Get Test Location",
                Address = "789 Get Ave"
            };
            var createResponse = await _api.CreateAsync(location);
            var locationId = createResponse.Content!.Id;

            // Act
            var getResponse = await _api.GetAsync(locationId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.Content.Should().NotBeNull();
            getResponse.Content!.Name.Should().Be("Get Test Location");
        }

        [Test]
        public async Task PatchServiceLocation_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Original Name",
                Address = "Original Address"
            };
            var createResponse = await _api.CreateAsync(location);
            var locationId = createResponse.Content!.Id;

            // Act
            var patchDoc = new JsonPatchDocument<Granum.Api.Features.ServiceLocation.ServiceLocation>();
            patchDoc.Replace(l => l.Name, "Updated Name");
            var patchResponse = await _api.UpdateAsync(locationId, patchDoc);

            // Assert
            patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify
            var getResponse = await _api.GetAsync(locationId);
            getResponse.Content!.Name.Should().Be("Updated Name");
            getResponse.Content.Address.Should().Be("Original Address");
        }

        [Test]
        public async Task DeleteServiceLocation_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Delete Test Location",
                Address = "101 Delete Ave"
            };
            var createResponse = await _api.CreateAsync(location);
            var locationId = createResponse.Content!.Id;

            // Act
            var deleteResponse = await _api.DeleteAsync(locationId);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify
            var getResponse = await _api.GetAsync(locationId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
