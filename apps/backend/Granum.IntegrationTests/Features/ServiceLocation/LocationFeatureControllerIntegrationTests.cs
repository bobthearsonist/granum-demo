using System.Net;
using FluentAssertions;
using Granum.Api.Features.ServiceLocation;
using Microsoft.AspNetCore.JsonPatch;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class LocationFeatureControllerIntegrationTests : IntegrationTestBase
    {
        private IServiceLocationApi _locationApi = null!;
        private ILocationFeatureApi _featureApi = null!;
        private const int TestCustomerId = 1;
        private int _testLocationId;

        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync()
        {
            // Initialize base and API clients
            base.OneTimeSetUp();
            _locationApi = CreateApi<IServiceLocationApi>();
            _featureApi = CreateApi<ILocationFeatureApi>();

            // Create a test service location for all tests
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Feature Test Location",
                Address = "123 Feature St"
            };

            var response = await _locationApi.CreateAsync(location);
            _testLocationId = response.Content!.Id;
        }

        [Test]
        public async Task CreateLocationFeature_WithValidData_ReturnsCreated()
        {
            // Arrange
            var feature = new LocationFeature
            {
                LocationId = _testLocationId,
                FeatureType = FeatureType.Trees,
                Measurement = 10.5f,
                Unit = MeasurementUnit.Count
            };

            // Act
            var response = await _featureApi.CreateAsync(_testLocationId, feature);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Content!.FeatureType.Should().Be(FeatureType.Trees);
            response.Content.Measurement.Should().Be(10.5f);
            response.Content.Unit.Should().Be(MeasurementUnit.Count);
        }

        [Test]
        public async Task GetAllLocationFeatures_ReturnsOkWithList()
        {
            // Act
            var response = await _featureApi.GetAllAsync(_testLocationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllLocationFeatures_AfterCreating_ReturnsListWithFeature()
        {
            // Arrange
            var feature = new LocationFeature
            {
                LocationId = _testLocationId,
                FeatureType = FeatureType.Driveway,
                Measurement = 500.0f,
                Unit = MeasurementUnit.SquareFeet
            };
            await _featureApi.CreateAsync(_testLocationId, feature);

            // Act
            var response = await _featureApi.GetAllAsync(_testLocationId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task GetLocationFeature_ByValidId_ReturnsOkWithFeature()
        {
            // Arrange
            var feature = new LocationFeature
            {
                LocationId = _testLocationId,
                FeatureType = FeatureType.Acreage,
                Measurement = 2.5f,
                Unit = MeasurementUnit.Acres,
                Description = "Main property acreage"
            };
            var createResponse = await _featureApi.CreateAsync(_testLocationId, feature);
            var featureId = createResponse.Content!.Id;

            // Act
            var getResponse = await _featureApi.GetAsync(_testLocationId, featureId);

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.Content.Should().NotBeNull();
            getResponse.Content!.FeatureType.Should().Be(FeatureType.Acreage);
            getResponse.Content.Measurement.Should().Be(2.5f);
            getResponse.Content.Description.Should().Be("Main property acreage");
        }

        [Test]
        public async Task UpdateLocationFeature_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var feature = new LocationFeature
            {
                LocationId = _testLocationId,
                FeatureType = FeatureType.Edges,
                Measurement = 100.0f,
                Unit = MeasurementUnit.LinearFeet
            };
            var createResponse = await _featureApi.CreateAsync(_testLocationId, feature);
            var featureId = createResponse.Content!.Id;

            // Act
            var patchDoc = new JsonPatchDocument<LocationFeature>();
            patchDoc.Replace(f => f.Measurement, 150.0f);
            patchDoc.Replace(f => f.Description, "Updated edges");
            var patchResponse = await _featureApi.UpdateAsync(_testLocationId, featureId, patchDoc);

            // Assert
            patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify
            var getResponse = await _featureApi.GetAsync(_testLocationId, featureId);
            getResponse.Content!.Measurement.Should().Be(150.0f);
            getResponse.Content.Description.Should().Be("Updated edges");
        }

        [Test]
        public async Task DeleteLocationFeature_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var feature = new LocationFeature
            {
                LocationId = _testLocationId,
                FeatureType = FeatureType.Beds,
                Measurement = 200.0f,
                Unit = MeasurementUnit.SquareFeet
            };
            var createResponse = await _featureApi.CreateAsync(_testLocationId, feature);
            var featureId = createResponse.Content!.Id;

            // Act
            var deleteResponse = await _featureApi.DeleteAsync(_testLocationId, featureId);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify
            var getResponse = await _featureApi.GetAsync(_testLocationId, featureId);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetLocationFeature_WithNonExistentId_ReturnsNotFound()
        {
            // Act
            var response = await _featureApi.GetAsync(_testLocationId, 999);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
