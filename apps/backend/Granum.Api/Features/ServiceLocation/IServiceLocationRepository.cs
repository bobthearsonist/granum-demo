using Granum.Api.Infrastructure;

namespace Granum.Api.Features.ServiceLocation;

public interface IServiceLocationRepository : IRepository<ServiceLocation>
{
    Task<IEnumerable<ServiceLocation>> GetByCustomerIdAsync(int customerId);
}
