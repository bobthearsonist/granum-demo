using FluentAssertions;
using Granum.Api.Features.User;

namespace Granum.IntegrationTests.Features.User
{
    [TestFixture]
    public class UserRepositoryIntegrationTests : RepositoryTestBase
    {
        private UserRepository<Customer> _customerRepository;
        private UserRepository<Contractor> _contractorRepository;

        [SetUp]
        public override async Task SetUpAsync()
        {
            await base.SetUpAsync();
            _customerRepository = new UserRepository<Customer>(DbContext);
            _contractorRepository = new UserRepository<Contractor>(DbContext);
        }

        [Test]
        public async Task AddAsync_WithValidCustomer_ReturnsAddedCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "John Customer" };

            // Act
            var result = await _customerRepository.AddAsync(customer);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("John Customer");
        }

        [Test]
        public async Task AddAsync_WithValidContractor_ReturnsAddedContractor()
        {
            // Arrange
            var contractor = new Contractor { Name = "Bob Contractor" };

            // Act
            var result = await _contractorRepository.AddAsync(contractor);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Bob Contractor");
        }

        [Test]
        public async Task GetByIdAsync_AfterAdding_ReturnsCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "Jane Customer" };
            var added = await _customerRepository.AddAsync(customer);

            // Act
            var retrieved = await _customerRepository.GetByIdAsync(added.Id);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(added.Id);
            retrieved.Name.Should().Be("Jane Customer");
        }

        [Test]
        public async Task GetAllAsync_AfterAddingMultiple_ReturnsAllCustomers()
        {
            // Arrange
            var customer1 = new Customer { Name = "Customer 1" };
            var customer2 = new Customer { Name = "Customer 2" };
            await _customerRepository.AddAsync(customer1);
            await _customerRepository.AddAsync(customer2);

            // Act
            var result = await _customerRepository.GetAllAsync();

            // Assert
            var customerList = result.ToList();
            customerList.Should().HaveCount(2);
            customerList.Should().Contain(c => c.Name == "Customer 1");
            customerList.Should().Contain(c => c.Name == "Customer 2");
        }

        [Test]
        public async Task UpdateAsync_WithModifiedCustomer_ReturnsUpdatedCustomer()
        {
            // Arrange
            var customer = new Customer { Name = "Original Name" };
            var added = await _customerRepository.AddAsync(customer);
            added.Name = "Updated Name";

            // Act
            var result = await _customerRepository.UpdateAsync(added);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Name");

            // Verify persistence
            var retrieved = await _customerRepository.GetByIdAsync(added.Id);
            retrieved!.Name.Should().Be("Updated Name");
        }

        [Test]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Act
            var result = await _customerRepository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_WithNoCustomers_ReturnsEmptyList()
        {
            // Act
            var result = await _customerRepository.GetAllAsync();

            // Assert
            var customerList = result.ToList();
            customerList.Should().BeEmpty();
        }
    }
}
