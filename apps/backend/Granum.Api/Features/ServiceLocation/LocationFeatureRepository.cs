using Granum.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Granum.Api.Features.ServiceLocation;

public class LocationFeatureRepository(IAppDbContext context) : ILocationFeatureRepository
{
    public async Task<IEnumerable<LocationFeature>> GetByLocationIdAsync(int locationId) =>
        await context.Set<LocationFeature>()
            .Where(x => x.LocationId == locationId)
            .ToListAsync();

    public Task<LocationFeature?> GetByIdAsync(int id) => context.Set<LocationFeature>().FindAsync(id).AsTask();

    public async Task<LocationFeature> AddAsync(LocationFeature feature)
    {
        var modified = context.Add(feature);
        await context.SaveChangesAsync();
        return modified.Entity;
    }

    public async Task<LocationFeature?> UpdateAsync(LocationFeature feature)
    {
        context.Update(feature);
        await context.SaveChangesAsync();
        return feature;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"LocationFeature with ID {id} not found.");

        context.Remove(entity);
        await context.SaveChangesAsync();
    }
}
