using Granum.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Granum.Api.Features.ServiceLocation;

public class ServiceLocationRepository(IAppDbContext context) : IServiceLocationRepository
{
    public Task<ServiceLocation?> GetByIdAsync(int id) => context.Set<ServiceLocation>().FindAsync(id).AsTask();

    public async Task<IEnumerable<ServiceLocation>> GetAllAsync() => await context.Set<ServiceLocation>().ToListAsync();

    public async Task<ServiceLocation> AddAsync(ServiceLocation entity)
    {
        var modified = context.Add(entity);
        await context.SaveChangesAsync();
        return modified.Entity;
    }

    public async Task<ServiceLocation?> UpdateAsync(ServiceLocation entity)
    {
        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<ServiceLocation>> GetByCustomerIdAsync(int customerId) =>
        await context.Set<ServiceLocation>()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync();

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"ServiceLocation with ID {id} not found.");
        
        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.Set<ServiceLocation>().AnyAsync(x => x.Id == id);
    }
}
