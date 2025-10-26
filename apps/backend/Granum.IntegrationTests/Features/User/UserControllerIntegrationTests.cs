using System.Net;
using FluentAssertions;
using Granum.IntegrationTests.Infrastructure;

namespace Granum.IntegrationTests.Features.User
{
    [TestFixture]
    public class UserControllerIntegrationTests : IntegrationTestBase
    {
        private const string CustomersUrl = "/api/customers";
        private const string ContractorsUrl = "/api/contractors";

        [Test]
        public async Task CreateCustomer_WithValidData_ReturnsCreated()
        {
            var createRequest = new { email = "customer@example.com", name = "John Customer" };

            var response = await PostJsonAsync(CustomersUrl, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateCustomer_WithoutEmail_ReturnsBadRequest()
        {
            var invalidRequest = new { email = (string?)null, name = "John Customer" };

            var response = await PostJsonAsync(CustomersUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateCustomer_WithDuplicateEmail_ReturnsBadRequest()
        {
            var createRequest = new { email = "duplicate@example.com", name = "John Customer" };

            await PostJsonAsync(CustomersUrl, createRequest);

            var response = await PostJsonAsync(CustomersUrl, createRequest);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
        }

        [Test]
        public async Task GetAllCustomers_ReturnsOkWithList()
        {
            var response = await GetAsync(CustomersUrl);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllCustomers_AfterCreatingCustomer_ReturnsListWithCustomer()
        {
            var createRequest = new { email = "customer1@example.com", name = "Customer 1" };
            await PostJsonAsync(CustomersUrl, createRequest);

            var response = await GetAsync(CustomersUrl);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task CreateContractor_WithValidData_ReturnsCreated()
        {
            var createRequest = new { email = "contractor@example.com", name = "Bob Contractor" };

            var response = await PostJsonAsync(ContractorsUrl, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateContractor_WithoutEmail_ReturnsBadRequest()
        {
            var invalidRequest = new { email = (string?)null, name = "Bob Contractor" };

            var response = await PostJsonAsync(ContractorsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateContractor_WithDuplicateEmail_ReturnsBadRequest()
        {
            var createRequest = new { email = "duplicate-contractor@example.com", name = "Bob Contractor" };

            await PostJsonAsync(ContractorsUrl, createRequest);

            var response = await PostJsonAsync(ContractorsUrl, createRequest);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
        }

        [Test]
        public async Task GetAllContractors_ReturnsOkWithList()
        {
            var response = await GetAsync(ContractorsUrl);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);
            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetAllContractors_AfterCreatingContractor_ReturnsListWithContractor()
        {
            var createRequest = new { email = "contractor1@example.com", name = "Contractor 1" };
            await PostJsonAsync(ContractorsUrl, createRequest);

            var response = await GetAsync(ContractorsUrl);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }
    }
}
