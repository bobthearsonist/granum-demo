using FluentAssertions;
using Granum.IntegrationTests.Generated;

namespace Granum.IntegrationTests.Features.UserTests
{
    [TestFixture]
    public class UserControllerIntegrationTests : IntegrationTestBase
    {
        private ICustomersApi _customerApi = null!;
        private IContractorsApi _contractorApi = null!;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            _customerApi = CreateApi<ICustomersApi>();
            _contractorApi = CreateApi<IContractorsApi>();
        }

        [Test]
        public async Task CreateCustomer_WithValidName_ReturnsCreated()
        {
            var createRequest = new CustomerCreate { Name = "John Customer" };

            var result = await _customerApi.CreateCustomer(createRequest);

            result.Should().NotBeNull();
            result.Name.Should().Be("John Customer");
        }

        [Test]
        public async Task CreateCustomer_WithoutName_ReturnsBadRequest()
        {
            var createRequest = new CustomerCreate { Name = null! };

            Func<Task> act = async () => await _customerApi.CreateCustomer(createRequest);

            await act.Should().ThrowAsync<Refit.ApiException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllCustomers_ReturnsOkWithList()
        {
            var result = await _customerApi.GetCustomers();

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllCustomers_AfterCreatingCustomer_ReturnsListWithCustomer()
        {
            var createRequest = new CustomerCreate { Name = "Customer 1" };
            await _customerApi.CreateCustomer(createRequest);

            var result = await _customerApi.GetCustomers();

            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task CreateContractor_WithValidName_ReturnsCreated()
        {
            var createRequest = new ContractorCreate { Name = "Bob Contractor" };

            var result = await _contractorApi.CreateContractor(createRequest);

            result.Should().NotBeNull();
            result.Name.Should().Be("Bob Contractor");
        }

        [Test]
        public async Task CreateContractor_WithoutName_ReturnsBadRequest()
        {
            var createRequest = new ContractorCreate { Name = null! };

            Func<Task> act = async () => await _contractorApi.CreateContractor(createRequest);

            await act.Should().ThrowAsync<Refit.ApiException>()
                .Where(e => e.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllContractors_ReturnsOkWithList()
        {
            var result = await _contractorApi.GetContractors();

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllContractors_AfterCreatingContractor_ReturnsListWithContractor()
        {
            var createRequest = new ContractorCreate { Name = "Contractor 1" };
            await _contractorApi.CreateContractor(createRequest);

            var result = await _contractorApi.GetContractors();

            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }
    }
}
