using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.ServiceLocation;

public class ServiceLocationService([FromServices] IServiceLocationRepository repository) : IServiceLocationService
{
    private const string ServiceLocationNotFoundFormat = "ServiceLocation with ID {0} not found.";

    public async Task<IEnumerable<ServiceLocation>> GetAllAsync() => await repository.GetAllAsync();

    public async Task<ServiceLocation> GetByIdAsync(int id) => await repository.GetByIdAsync(id) ?? ServiceLocationNotFoundException(id);

    public async Task<IEnumerable<ServiceLocation>> GetByCustomerIdAsync(int customerId) => await repository.GetByCustomerIdAsync(customerId);

    public async Task<ServiceLocation> CreateAsync(ServiceLocation location) => await repository.AddAsync(location);

    public async Task<ServiceLocation?> UpdateAsync(ServiceLocation location)
    {
        return await repository.UpdateAsync(location) ?? ServiceLocationNotFoundException(location.Id);
    }

    public async Task DeleteAsync(int id)
    {
        await repository.DeleteAsync(id);
    }

    private static ServiceLocation ServiceLocationNotFoundException(int id)
    {
        throw new KeyNotFoundException(string.Format(ServiceLocationNotFoundFormat, id));
    }
}
