using FluentAssertions;
using Granum.Api.Features.ServiceLocation;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class ServiceLocationRepositoryIntegrationTests : RepositoryTestBase
    {
        private ServiceLocationRepository _repository;
        private const int TestCustomerId = 1;

        [SetUp]
        public override async Task SetUpAsync()
        {
            await base.SetUpAsync();
            _repository = new ServiceLocationRepository(DbContext);
        }

        [Test]
        public async Task AddAsync_WithValidServiceLocation_ReturnsAddedLocation()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Main Office",
                Address = "123 Main St"
            };

            // Act
            var result = await _repository.AddAsync(location);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Main Office");
            result.Address.Should().Be("123 Main St");
            result.CustomerId.Should().Be(TestCustomerId);
        }

        [Test]
        public async Task GetByIdAsync_AfterAdding_ReturnsLocation()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Warehouse",
                Address = "456 Warehouse Ave"
            };
            var added = await _repository.AddAsync(location);

            // Act
            var retrieved = await _repository.GetByIdAsync(added.Id);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(added.Id);
            retrieved.Name.Should().Be("Warehouse");
            retrieved.Address.Should().Be("456 Warehouse Ave");
        }

        [Test]
        public async Task GetAllAsync_AfterAddingMultiple_ReturnsAllLocations()
        {
            // Arrange
            var location1 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Location 1",
                Address = "Address 1"
            };
            var location2 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Location 2",
                Address = "Address 2"
            };
            await _repository.AddAsync(location1);
            await _repository.AddAsync(location2);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            var locationList = result.ToList();
            locationList.Should().HaveCount(2);
            locationList.Should().Contain(l => l.Name == "Location 1");
            locationList.Should().Contain(l => l.Name == "Location 2");
        }

        [Test]
        public async Task UpdateAsync_WithModifiedLocation_ReturnsUpdatedLocation()
        {
            // Arrange
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Original Name",
                Address = "Original Address"
            };
            var added = await _repository.AddAsync(location);
            added.Name = "Updated Name";
            added.Address = "Updated Address";

            // Act
            var result = await _repository.UpdateAsync(added);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Name");
            result.Address.Should().Be("Updated Address");

            // Verify persistence
            var retrieved = await _repository.GetByIdAsync(added.Id);
            retrieved!.Name.Should().Be("Updated Name");
            retrieved.Address.Should().Be("Updated Address");
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
        public async Task GetAllAsync_WithNoLocations_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            var locationList = result.ToList();
            locationList.Should().BeEmpty();
        }

        [Test]
        public async Task GetByCustomerIdAsync_WithValidCustomerId_ReturnsCustomerLocations()
        {
            // Arrange
            var location1 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Customer 1 Location 1",
                Address = "Address 1"
            };
            var location2 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = TestCustomerId,
                Name = "Customer 1 Location 2",
                Address = "Address 2"
            };
            var location3 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = 2,
                Name = "Customer 2 Location",
                Address = "Address 3"
            };
            await _repository.AddAsync(location1);
            await _repository.AddAsync(location2);
            await _repository.AddAsync(location3);

            // Act
            var result = await _repository.GetByCustomerIdAsync(TestCustomerId);

            // Assert
            var locationList = result.ToList();
            locationList.Should().HaveCount(2);
            locationList.Should().AllSatisfy(l => l.CustomerId.Should().Be(TestCustomerId));
            locationList.Should().Contain(l => l.Name == "Customer 1 Location 1");
            locationList.Should().Contain(l => l.Name == "Customer 1 Location 2");
        }

        [Test]
        public async Task GetByCustomerIdAsync_WithNonExistentCustomerId_ReturnsEmptyList()
        {
            // Act
            var result = await _repository.GetByCustomerIdAsync(999);

            // Assert
            var locationList = result.ToList();
            locationList.Should().BeEmpty();
        }
    }
}
