using Granum.Api.Features.ServiceLocation;
using Granum.Api.Infrastructure;
using NSubstitute;

namespace Granum.Tests.Features.ServiceLocation;

/* Test coverage is limited to methods with logic beyond pure delegation to the repository.
 Methods that simply delegate calls to the repository are marked with [Ignore] attribute and left unimplemented. They should be covered by integration tests. */
[TestFixture]
public class ServiceLocationRepositoryTests
{
    private IServiceLocationRepository _repository;
    private readonly IAppDbContext _context = Substitute.For<IAppDbContext>();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _repository = new ServiceLocationRepository(_context);
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetAllAsync() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetByIdAsync() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task AddAsync() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task UpdateAsync() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetByCustomerIdAsync() => Task.CompletedTask;
}
