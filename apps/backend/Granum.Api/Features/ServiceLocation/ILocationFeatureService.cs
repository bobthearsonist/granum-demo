namespace Granum.Api.Features.ServiceLocation;

public interface ILocationFeatureService
{
    Task<IEnumerable<LocationFeature>> GetByLocationIdAsync(int locationId);
    Task<LocationFeature> GetByIdAsync(int id);
    Task<LocationFeature> CreateAsync(LocationFeature feature);
    Task<LocationFeature?> UpdateAsync(LocationFeature feature);
    Task DeleteAsync(int id);
}
