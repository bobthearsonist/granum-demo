using FluentAssertions;
using Granum.Api.Features.User;
using NSubstitute;

namespace Granum.Tests.Features.User;

[TestFixture(typeof(Customer))]
public class UserServiceTests<TUser>
    where TUser : Api.Features.User.User, new()
{
    private IUserRepository<TUser> _mockUserRepository;
    private UserService<TUser> _userService;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _mockUserRepository = Substitute.For<IUserRepository<TUser>>();
        _userService = new UserService<TUser>(_mockUserRepository);
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetAllAsync_CallsRepository() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task CreateAsync_CallsRepository() => Task.CompletedTask;

    [Test]
    public async Task GetByIdAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        _mockUserRepository.GetByIdAsync(0).Returns((TUser?)null!);

        await _userService.Invoking(s => s.GetByIdAsync(0))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("User with ID 0 not found.");
    }

    [Test]
    public async Task UpdateAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        var user = new TUser { Id = 1 };
        _mockUserRepository.UpdateAsync(user).Returns((TUser?)null!);

        await _userService.Invoking(s => s.UpdateAsync(user))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("User with ID 1 not found.");
    }
}
