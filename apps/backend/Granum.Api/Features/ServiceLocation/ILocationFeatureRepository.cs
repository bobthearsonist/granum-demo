namespace Granum.Api.Features.ServiceLocation;

public interface ILocationFeatureRepository
{
    Task<IEnumerable<LocationFeature>> GetByLocationIdAsync(int locationId);
    Task<LocationFeature?> GetByIdAsync(int id);
    Task<LocationFeature> AddAsync(LocationFeature feature);
    Task<LocationFeature?> UpdateAsync(LocationFeature feature);
    Task DeleteAsync(int id);
}
