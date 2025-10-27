using FluentAssertions;
using Granum.IntegrationTests.Generated;
using Microsoft.AspNetCore.JsonPatch;
using Refit;
using FeatureType = Granum.IntegrationTests.Generated.FeatureType;
using MeasurementUnit = Granum.IntegrationTests.Generated.MeasurementUnit;

namespace Granum.IntegrationTests.Features.ServiceLocationTests
{
    [TestFixture]
    public class LocationFeatureControllerIntegrationTests : IntegrationTestBase
    {
        private IServiceLocationsApi _locationApi = null!;
        private ILocationFeaturesApi _featureApi = null!;
        private const int TestCustomerId = 1;
        private int _testLocationId;

        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync()
        {
            // Initialize base and API clients
            base.OneTimeSetUp();
            _locationApi = CreateApi<IServiceLocationsApi>();
            _featureApi = CreateApi<ILocationFeaturesApi>();

            // Create a test service location for all tests
            var createRequest = new ServiceLocationCreate
            {
                CustomerId = TestCustomerId,
                Name = "Feature Test Location",
                Address = "123 Feature St"
            };

            var location = await _locationApi.CreateServiceLocation(createRequest);
            _testLocationId = location.Id;
        }

        [Test]
        public async Task CreateLocationFeature_WithValidData_ReturnsCreated()
        {
            // Arrange
            var createRequest = new LocationFeatureCreate
            {
                FeatureType = FeatureType.Trees,
                Measurement = 10.5f,
                Unit = MeasurementUnit.Count
            };

            // Act
            var result = await _featureApi.CreateLocationFeature(_testLocationId, createRequest);

            // Assert
            result.Should().NotBeNull();
            result.FeatureType.Should().Be(FeatureType.Trees);
            result.Measurement.Should().Be(10.5f);
            result.Unit.Should().Be(MeasurementUnit.Count);
        }

        [Test]
        public async Task GetAllLocationFeatures_ReturnsOkWithList()
        {
            // Act
            var result = await _featureApi.GetLocationFeatures(_testLocationId);

            // Assert
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllLocationFeatures_AfterCreating_ReturnsListWithFeature()
        {
            // Arrange
            var createRequest = new LocationFeatureCreate
            {
                FeatureType = FeatureType.Driveway,
                Measurement = 500.0f,
                Unit = MeasurementUnit.SquareFeet
            };
            await _featureApi.CreateLocationFeature(_testLocationId, createRequest);

            // Act
            var result = await _featureApi.GetLocationFeatures(_testLocationId);

            // Assert
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task GetLocationFeature_ByValidId_ReturnsOkWithFeature()
        {
            // Arrange
            var createRequest = new LocationFeatureCreate
            {
                FeatureType = FeatureType.Acreage,
                Measurement = 2.5f,
                Unit = MeasurementUnit.Acres,
                Description = "Main property acreage"
            };
            var created = await _featureApi.CreateLocationFeature(_testLocationId, createRequest);
            var featureId = created.Id;

            // Act
            var result = await _featureApi.GetLocationFeature(_testLocationId, featureId);

            // Assert
            result.Should().NotBeNull();
            result.FeatureType.Should().Be(FeatureType.Acreage);
            result.Measurement.Should().Be(2.5f);
            result.Description.Should().Be("Main property acreage");
        }

        [Test]
        public async Task UpdateLocationFeature_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var createRequest = new LocationFeatureCreate
            {
                FeatureType = FeatureType.Edges,
                Measurement = 100.0f,
                Unit = MeasurementUnit.LinearFeet
            };
            var created = await _featureApi.CreateLocationFeature(_testLocationId, createRequest);
            var featureId = created.Id;

            // Act
            var patchDoc = new JsonPatchDocument<LocationFeature>();
            patchDoc.Replace(f => f.Measurement, 150.0f);
            patchDoc.Replace(f => f.Description, "Updated edges");
            await _featureApi.UpdateLocationFeature(_testLocationId, featureId, patchDoc.Operations);

            // Verify
            var updated = await _featureApi.GetLocationFeature(_testLocationId, featureId);
            updated.Measurement.Should().Be(150.0f);
            updated.Description.Should().Be("Updated edges");
        }

        [Test]
        public async Task DeleteLocationFeature_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var createRequest = new LocationFeatureCreate
            {
                FeatureType = FeatureType.Beds,
                Measurement = 200.0f,
                Unit = MeasurementUnit.SquareFeet
            };
            var created = await _featureApi.CreateLocationFeature(_testLocationId, createRequest);
            var featureId = created.Id;

            // Act
            await _featureApi.DeleteLocationFeature(_testLocationId, featureId);

            // Verify - should throw Refit.ApiException with NotFound status
            var getAction = async () => await _featureApi.GetLocationFeature(_testLocationId, featureId);
            await getAction.Should().ThrowAsync<Refit.ApiException>();
        }

        [Test]
        public async Task GetLocationFeature_WithNonExistentId_ReturnsNotFound()
        {
            // Act
            var action = async () => await _featureApi.GetLocationFeature(_testLocationId, 999);

            // Assert
            await action.Should().ThrowAsync<Refit.ApiException>();
        }
    }
}
