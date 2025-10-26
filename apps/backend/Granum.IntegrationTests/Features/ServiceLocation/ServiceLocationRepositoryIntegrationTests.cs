using FluentAssertions;
using Granum.Api.Features.ServiceLocation;
using Granum.Api.Features.User;

namespace Granum.IntegrationTests.Features.ServiceLocation
{
    [TestFixture]
    public class ServiceLocationRepositoryIntegrationTests : RepositoryTestBase
    {
        private ServiceLocationRepository _repository;

        [SetUp]
        public override async Task SetUpAsync()
        {
            await base.SetUpAsync();
            _repository = new ServiceLocationRepository(DbContext);
        }

        private async Task<int> CreateTestCustomer(string name = "Test Customer")
        {
            var customer = new Customer { Name = name };
            DbContext.Customers.Add(customer);
            await DbContext.SaveChangesAsync();
            return customer.Id;
        }

        [Test]
        public async Task AddAsync_WithValidServiceLocation_ReturnsAddedLocation()
        {
            // Arrange
            var customerId = await CreateTestCustomer();
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customerId,
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
            result.CustomerId.Should().Be(customerId);
        }

        [Test]
        public async Task GetByIdAsync_AfterAdding_ReturnsLocation()
        {
            // Arrange
            var customerId = await CreateTestCustomer();
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customerId,
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
            var customerId = await CreateTestCustomer();
            var location1 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customerId,
                Name = "Location 1",
                Address = "Address 1"
            };
            var location2 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customerId,
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
            var customerId = await CreateTestCustomer();
            var location = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customerId,
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
            var customer1Id = await CreateTestCustomer("Customer 1");
            var customer2Id = await CreateTestCustomer("Customer 2");
            
            var location1 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customer1Id,
                Name = "Customer 1 Location 1",
                Address = "Address 1"
            };
            var location2 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customer1Id,
                Name = "Customer 1 Location 2",
                Address = "Address 2"
            };
            var location3 = new Granum.Api.Features.ServiceLocation.ServiceLocation
            {
                CustomerId = customer2Id,
                Name = "Customer 2 Location",
                Address = "Address 3"
            };
            await _repository.AddAsync(location1);
            await _repository.AddAsync(location2);
            await _repository.AddAsync(location3);

            // Act
            var result = await _repository.GetByCustomerIdAsync(customer1Id);

            // Assert
            var locationList = result.ToList();
            locationList.Should().HaveCount(2);
            locationList.Should().AllSatisfy(l => l.CustomerId.Should().Be(customer1Id));
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
