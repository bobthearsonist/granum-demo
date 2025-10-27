using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.ServiceLocation;

public class LocationFeatureService([FromServices] ILocationFeatureRepository repository) : ILocationFeatureService
{
    private const string LocationFeatureNotFoundFormat = "LocationFeature with ID {0} not found.";

    public async Task<IEnumerable<LocationFeature>> GetByLocationIdAsync(int locationId) =>
        await repository.GetByLocationIdAsync(locationId);

    public async Task<LocationFeature> GetByIdAsync(int id) =>
        await repository.GetByIdAsync(id) ?? LocationFeatureNotFoundException(id);

    public async Task<LocationFeature> CreateAsync(LocationFeature feature) =>
        await repository.AddAsync(feature);

    public async Task<LocationFeature?> UpdateAsync(LocationFeature feature)
    {
        return await repository.UpdateAsync(feature) ?? LocationFeatureNotFoundException(feature.Id);
    }

    public async Task DeleteAsync(int id)
    {
        await repository.DeleteAsync(id);
    }

    private static LocationFeature LocationFeatureNotFoundException(int id)
    {
        throw new KeyNotFoundException(string.Format(LocationFeatureNotFoundFormat, id));
    }
}
