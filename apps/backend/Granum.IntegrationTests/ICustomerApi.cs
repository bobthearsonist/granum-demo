using Granum.Api.Features.User;
using Microsoft.AspNetCore.JsonPatch;
using Refit;

namespace Granum.IntegrationTests;

public interface ICustomerApi
{
    [Get("/api/customers")]
    Task<IApiResponse<List<Customer>>> GetAllAsync();

    [Get("/api/customers/{id}")]
    Task<IApiResponse<Customer>> GetAsync(int id);

    [Post("/api/customers")]
    Task<IApiResponse<Customer>> CreateAsync([Body] Customer customer);

    [Patch("/api/customers/{id}")]
    Task<IApiResponse> UpdateAsync(int id, [Body] JsonPatchDocument<Customer> patchDoc);
}
