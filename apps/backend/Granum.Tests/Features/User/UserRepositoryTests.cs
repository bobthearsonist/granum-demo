using FluentAssertions;
using Granum.Api.Features.User;
using Granum.Api.Infrastructure;
using NSubstitute;

namespace Granum.Tests.Features.User;

/* Test coverage is limited to methods with logic beyond pure delegation to the repository.
 Methods that simply delegate calls to the repository are marked with [Ignore] attribute and left unimplemented. They should be covered by integration tests. */
[TestFixture(typeof(Customer))]
[TestFixture(typeof(Contractor))]
public abstract class UserRepositoryTests<TUser>
    where TUser : Api.Features.User.User
{
    private IUserRepository<TUser> _userRepository;
    private readonly AppDbContext _context = Substitute.ForPartsOf<AppDbContext>();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _userRepository = new UserRepository<TUser>(_context);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _context.Dispose();
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetAllAsync() => Task.CompletedTask;

    [Test]
    public async Task GetAllAsync_WhenNotFound_ReturnsNull()
    {
        _context.Set<TUser>().FindAsync(Arg.Any<int>()).Returns((TUser?)null!);

        var result = await _userRepository.GetByIdAsync(1);

        result.Should().BeNull();
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task AddAsync() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task UpdateAsync() => Task.CompletedTask;
}
