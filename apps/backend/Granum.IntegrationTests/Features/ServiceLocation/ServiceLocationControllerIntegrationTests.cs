using FluentAssertions;
using Granum.IntegrationTests.Generated;
using Microsoft.AspNetCore.JsonPatch;

namespace Granum.IntegrationTests.Features.ServiceLocationTests
{
    [TestFixture]
    public class ServiceLocationControllerIntegrationTests : IntegrationTestBase
    {
        private IServiceLocationsApi _api = null!;
        private const int TestCustomerId = 1;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            _api = CreateApi<IServiceLocationsApi>();
        }

        [Test]
        public async Task CreateServiceLocation_WithValidData_ReturnsCreated()
        {
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Main Office",
                Address = "123 Main St"
            };

            var result = await _api.CreateServiceLocation(createRequest);

            result.Should().NotBeNull();
            result.Name.Should().Be("Main Office");
        }

        [Test]
        public async Task CreateServiceLocation_WithoutName_ReturnsBadRequest()
        {
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = null!,
                Address = "123 Main St"
            };

            Func<Task> act = async () => await _api.CreateServiceLocation(createRequest);

            await act.Should().ThrowAsync<Refit.ApiException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateServiceLocation_WithoutAddress_ReturnsBadRequest()
        {
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Main Office",
                Address = null!
            };

            Func<Task> act = async () => await _api.CreateServiceLocation(createRequest);

            await act.Should().ThrowAsync<Refit.ApiException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllServiceLocations_ReturnsOkWithList()
        {
            var result = await _api.GetServiceLocations();

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllServiceLocations_AfterCreating_ReturnsListWithLocation()
        {
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Test Location",
                Address = "456 Test Ave"
            };
            await _api.CreateServiceLocation(createRequest);

            var result = await _api.GetServiceLocations();

            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task GetServiceLocation_ByValidId_ReturnsOkWithLocation()
        {
            // Arrange
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Get Test Location",
                Address = "789 Get Ave"
            };
            var created = await _api.CreateServiceLocation(createRequest);
            var locationId = created.Id;

            // Act
            var result = await _api.GetServiceLocation(locationId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Get Test Location");
        }

        [Test]
        public async Task PatchServiceLocation_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Original Name",
                Address = "Original Address"
            };
            var created = await _api.CreateServiceLocation(createRequest);
            var locationId = created.Id;

            // Act
            var patchDoc = new JsonPatchDocument<ServiceLocationCreate>();
            patchDoc.Replace(l => l.Name, "Updated Name");
            await _api.UpdateServiceLocation(locationId, patchDoc.Operations);

            // Verify
            var result = await _api.GetServiceLocation(locationId);
            result.Name.Should().Be("Updated Name");
            result.Address.Should().Be("Original Address");
        }

        [Test]
        public async Task DeleteServiceLocation_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Delete Test Location",
                Address = "101 Delete Ave"
            };
            var created = await _api.CreateServiceLocation(createRequest);
            var locationId = created.Id;

            // Act
            await _api.DeleteServiceLocation(locationId);

            // Verify
            Func<Task> act = async () => await _api.GetServiceLocation(locationId);
            await act.Should().ThrowAsync<Refit.ApiException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}
