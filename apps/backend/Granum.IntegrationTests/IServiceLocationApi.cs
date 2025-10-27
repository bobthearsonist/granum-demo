using Microsoft.AspNetCore.JsonPatch;
using Refit;

namespace Granum.IntegrationTests;

public interface IServiceLocationApi
{
    [Get("/api/service-locations")]
    Task<IApiResponse<List<Granum.Api.Features.ServiceLocation.ServiceLocation>>> GetAllAsync();

    [Get("/api/service-locations/{id}")]
    Task<IApiResponse<Granum.Api.Features.ServiceLocation.ServiceLocation>> GetAsync(int id);

    [Post("/api/service-locations")]
    Task<IApiResponse<Granum.Api.Features.ServiceLocation.ServiceLocation>> CreateAsync([Body] Granum.Api.Features.ServiceLocation.ServiceLocation location);

    [Patch("/api/service-locations/{id}")]
    Task<IApiResponse> UpdateAsync(int id, [Body] JsonPatchDocument<Granum.Api.Features.ServiceLocation.ServiceLocation> patchDoc);

    [Delete("/api/service-locations/{id}")]
    Task<IApiResponse> DeleteAsync(int id);
}
