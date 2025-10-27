using System.Net;
using FluentAssertions;
using Granum.Api.Features.User;

namespace Granum.IntegrationTests.Features.User
{
    [TestFixture]
    public class UserControllerIntegrationTests : IntegrationTestBase
    {
        private ICustomerApi _customerApi = null!;
        private IContractorApi _contractorApi = null!;

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            _customerApi = CreateApi<ICustomerApi>();
            _contractorApi = CreateApi<IContractorApi>();
        }

        [Test]
        public async Task CreateCustomer_WithValidName_ReturnsCreated()
        {
            var customer = new Customer { Name = "John Customer" };

            var response = await _customerApi.CreateAsync(customer);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateCustomer_WithoutName_ReturnsBadRequest()
        {
            var customer = new Customer { Name = null! };

            var response = await _customerApi.CreateAsync(customer);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllCustomers_ReturnsOkWithList()
        {
            var response = await _customerApi.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllCustomers_AfterCreatingCustomer_ReturnsListWithCustomer()
        {
            var customer = new Customer { Name = "Customer 1" };
            await _customerApi.CreateAsync(customer);

            var response = await _customerApi.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task CreateContractor_WithValidName_ReturnsCreated()
        {
            var contractor = new Contractor { Name = "Bob Contractor" };

            var response = await _contractorApi.CreateAsync(contractor);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateContractor_WithoutName_ReturnsBadRequest()
        {
            var contractor = new Contractor { Name = null! };

            var response = await _contractorApi.CreateAsync(contractor);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetAllContractors_ReturnsOkWithList()
        {
            var response = await _contractorApi.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllContractors_AfterCreatingContractor_ReturnsListWithContractor()
        {
            var contractor = new Contractor { Name = "Contractor 1" };
            await _contractorApi.CreateAsync(contractor);

            var response = await _contractorApi.GetAllAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().HaveCountGreaterThanOrEqualTo(1);
        }
    }
}
