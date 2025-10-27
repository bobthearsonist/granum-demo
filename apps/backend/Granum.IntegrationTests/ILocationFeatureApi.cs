using Granum.Api.Features.ServiceLocation;
using Microsoft.AspNetCore.JsonPatch;
using Refit;

namespace Granum.IntegrationTests;

public interface ILocationFeatureApi
{
    [Get("/api/service-locations/{locationId}/features")]
    Task<IApiResponse<List<LocationFeature>>> GetAllAsync(int locationId);

    [Get("/api/service-locations/{locationId}/features/{featureId}")]
    Task<IApiResponse<LocationFeature>> GetAsync(int locationId, int featureId);

    [Post("/api/service-locations/{locationId}/features")]
    Task<IApiResponse<LocationFeature>> CreateAsync(int locationId, [Body] LocationFeature feature);

    [Patch("/api/service-locations/{locationId}/features/{featureId}")]
    Task<IApiResponse> UpdateAsync(int locationId, int featureId, [Body] JsonPatchDocument<LocationFeature> patchDoc);

    [Delete("/api/service-locations/{locationId}/features/{featureId}")]
    Task<IApiResponse> DeleteAsync(int locationId, int featureId);
}
