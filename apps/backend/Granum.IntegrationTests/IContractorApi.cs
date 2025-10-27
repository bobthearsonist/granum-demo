using Granum.Api.Features.User;
using Microsoft.AspNetCore.JsonPatch;
using Refit;

namespace Granum.IntegrationTests;

public interface IContractorApi
{
    [Get("/api/contractors")]
    Task<IApiResponse<List<Contractor>>> GetAllAsync();

    [Get("/api/contractors/{id}")]
    Task<IApiResponse<Contractor>> GetAsync(int id);

    [Post("/api/contractors")]
    Task<IApiResponse<Contractor>> CreateAsync([Body] Contractor contractor);

    [Patch("/api/contractors/{id}")]
    Task<IApiResponse> UpdateAsync(int id, [Body] JsonPatchDocument<Contractor> patchDoc);
}
