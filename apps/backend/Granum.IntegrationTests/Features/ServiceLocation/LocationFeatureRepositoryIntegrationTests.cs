using FluentAssertions;
using Granum.Api.Features.ServiceLocation;
using Granum.Api.Features.User;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class LocationFeatureRepositoryIntegrationTests : RepositoryTestBase
    {
        private LocationFeatureRepository _repository;
        private ServiceLocationRepository _serviceLocationRepository;

        [SetUp]
        public override async Task SetUpAsync()
        {
            await base.SetUpAsync();
            _repository = new LocationFeatureRepository(DbContext);
            _serviceLocationRepository = new ServiceLocationRepository(DbContext);
        }

        private async Task<int> CreateTestServiceLocation(string name = "Test Location")
        {
            var customer = new Customer { Name = "Test Customer" };
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();

            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customer.Id,
                Name = name,
                Address = "123 Test St"
            };
            var added = await _serviceLocationRepository.AddAsync(location);
            return added.Id;
        }

        [Test]
        public async Task AddAsync_WithValidLocationFeature_ReturnsAddedFeature()
        {
            // Arrange
            var locationId = await CreateTestServiceLocation();
            var feature = new LocationFeature
            {
                LocationId = locationId,
                FeatureType = FeatureType.Trees,
                Measurement = 10.5f,
                Unit = MeasurementUnit.Count
            };

            // Act
            var result = await _repository.AddAsync(feature);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.LocationId.Should().Be(locationId);
            result.FeatureType.Should().Be(FeatureType.Trees);
            result.Measurement.Should().Be(10.5f);
            result.Unit.Should().Be(MeasurementUnit.Count);
        }

        [Test]
        public async Task GetByIdAsync_AfterAdding_ReturnsFeature()
        {
            // Arrange
            var locationId = await CreateTestServiceLocation();
            var feature = new LocationFeature
            {
                LocationId = locationId,
                FeatureType = FeatureType.Driveway,
                Measurement = 500.0f,
                Unit = MeasurementUnit.SquareFeet,
                Description = "Front driveway"
            };
            var added = await _repository.AddAsync(feature);

            // Act
            var retrieved = await _repository.GetByIdAsync(added.Id);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(added.Id);
            retrieved.LocationId.Should().Be(locationId);
            retrieved.FeatureType.Should().Be(FeatureType.Driveway);
            retrieved.Measurement.Should().Be(500.0f);
            retrieved.Unit.Should().Be(MeasurementUnit.SquareFeet);
            retrieved.Description.Should().Be("Front driveway");
        }

        [Test]
        public async Task GetByLocationIdAsync_AfterAddingMultiple_ReturnsAllFeaturesForLocation()
        {
            // Arrange
            var location1Id = await CreateTestServiceLocation("Location 1");
            var location2Id = await CreateTestServiceLocation("Location 2");

            var feature1 = new LocationFeature
            {
                LocationId = location1Id,
                FeatureType = FeatureType.Trees,
                Measurement = 5.0f,
                Unit = MeasurementUnit.Count
            };
            var feature2 = new LocationFeature
            {
                LocationId = location1Id,
                FeatureType = FeatureType.Acreage,
                Measurement = 2.5f,
                Unit = MeasurementUnit.Acres
            };
            var feature3 = new LocationFeature
            {
                LocationId = location2Id,
                FeatureType = FeatureType.Beds,
                Measurement = 100.0f,
                Unit = MeasurementUnit.SquareFeet
            };
            await _repository.AddAsync(feature1);
            await _repository.AddAsync(feature2);
            await _repository.AddAsync(feature3);

            // Act
            var result = await _repository.GetByLocationIdAsync(location1Id);

            // Assert
            var featureList = result.ToList();
            featureList.Should().HaveCount(2);
            featureList.Should().AllSatisfy(f => f.LocationId.Should().Be(location1Id));
            featureList.Should().Contain(f => f.FeatureType == FeatureType.Trees);
            featureList.Should().Contain(f => f.FeatureType == FeatureType.Acreage);
        }

        [Test]
        public async Task UpdateAsync_WithModifiedFeature_ReturnsUpdatedFeature()
        {
            // Arrange
            var locationId = await CreateTestServiceLocation();
            var feature = new LocationFeature
            {
                LocationId = locationId,
                FeatureType = FeatureType.Edges,
                Measurement = 100.0f,
                Unit = MeasurementUnit.LinearFeet
            };
            var added = await _repository.AddAsync(feature);
            added.Measurement = 150.0f;
            added.Description = "Updated edges";

            // Act
            var result = await _repository.UpdateAsync(added);

            // Assert
            result.Should().NotBeNull();
            result!.Measurement.Should().Be(150.0f);
            result.Description.Should().Be("Updated edges");

            // Verify persistence
            var retrieved = await _repository.GetByIdAsync(added.Id);
            retrieved!.Measurement.Should().Be(150.0f);
            retrieved.Description.Should().Be("Updated edges");
        }

        [Test]
        public async Task DeleteAsync_WithValidFeatureId_RemovesFeature()
        {
            // Arrange
            var locationId = await CreateTestServiceLocation();
            var feature = new LocationFeature
            {
                LocationId = locationId,
                FeatureType = FeatureType.Trees,
                Measurement = 5.0f,
                Unit = MeasurementUnit.Count
            };
            var added = await _repository.AddAsync(feature);

            // Act
            await _repository.DeleteAsync(added.Id);

            // Assert
            var retrieved = await _repository.GetByIdAsync(added.Id);
            retrieved.Should().BeNull();
        }

        [Test]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetByLocationIdAsync_WithNonExistentLocationId_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetByLocationIdAsync(999);

            // Assert
            var featureList = result.ToList();
            featureList.Should().BeEmpty();
        }

        [Test]
        public async Task DeleteAsync_WithNonExistentId_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            await _repository.Invoking(r => r.DeleteAsync(999))
                .Should()
                .ThrowAsync<KeyNotFoundException>();
        }
    }
}
