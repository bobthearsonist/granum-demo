using System.Net;
using FluentAssertions;

namespace Granum.IntegrationTests.Features.User
{
    [TestFixture]
    public class UserControllerIntegrationTests : IntegrationTestBase
    {
        private const string CustomersUrl = "/api/customers";
        private const string ContractorsUrl = "/api/contractors";

        [Test]
        public async Task CreateCustomer_WithValidName_ReturnsCreated()
        {
            var createRequest = new { name = "John Customer" };

            var response = await PostJsonAsync(CustomersUrl, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateCustomer_WithoutName_ReturnsBadRequest()
        {
            var invalidRequest = new { name = (string?)null };

            var response = await PostJsonAsync(CustomersUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateCustomer_WithInvalidEmailProperty_ReturnsBadRequest()
        {
            var invalidRequest = new { email = "customer@example.com", name = "John Customer" };

            var response = await PostJsonAsync(CustomersUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
            var createRequest = new { name = "Customer 1" };
            await PostJsonAsync(CustomersUrl, createRequest);

            var response = await GetAsync(CustomersUrl);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task CreateContractor_WithValidName_ReturnsCreated()
        {
            var createRequest = new { name = "Bob Contractor" };

            var response = await PostJsonAsync(ContractorsUrl, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        public async Task CreateContractor_WithoutName_ReturnsBadRequest()
        {
            var invalidRequest = new { name = (string?)null };

            var response = await PostJsonAsync(ContractorsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task CreateContractor_WithInvalidEmailProperty_ReturnsBadRequest()
        {
            var invalidRequest = new { email = "contractor@example.com", name = "Bob Contractor" };

            var response = await PostJsonAsync(ContractorsUrl, invalidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
            var createRequest = new { name = "Contractor 1" };
            await PostJsonAsync(ContractorsUrl, createRequest);

            var response = await GetAsync(ContractorsUrl);
            var result = await DeserializeResponseAsync<List<dynamic>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().HaveCountGreaterThanOrEqualTo(1);
        }

    }
}
