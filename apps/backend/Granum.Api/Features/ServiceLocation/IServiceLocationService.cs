namespace Granum.Api.Features.ServiceLocation;

public interface IServiceLocationService
{
    Task<IEnumerable<ServiceLocation>> GetAllAsync();
    Task<ServiceLocation> GetByIdAsync(int id);
    Task<IEnumerable<ServiceLocation>> GetByCustomerIdAsync(int customerId);
    Task<ServiceLocation> CreateAsync(ServiceLocation location);
    Task<ServiceLocation?> UpdateAsync(ServiceLocation location);
    Task DeleteAsync(int id);
}
