using FluentAssertions;
using Granum.Api.Features.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Granum.Tests.Features.User;

[TestFixture(typeof(Customer), typeof(CustomersController))]
[TestFixture(typeof(Contractor), typeof(ContractorsController))]
public class UserControllerBaseTests<TUser, TController>
    where TUser : Api.Features.User.User, new()
    where TController : UserControllerBase<TUser>
{
    private IUserService<Api.Features.User.User> _mockUserService;
    private ILogger _mockLogger;
    private UserControllerBase<Api.Features.User.User> _controller;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _mockUserService = Substitute.For<IUserService<Api.Features.User.User>>();
        _mockLogger = Substitute.For<ILogger>();
        _controller = Substitute.ForPartsOf<UserControllerBase<Api.Features.User.User>>(
            _mockUserService,
            _mockLogger
        );
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task GetAllUsers_ReturnsListOfUsers()
    {
        var users = new[] { new TUser{Id = 1, Name = "bob"}, new TUser{Id = 2, Name = "susan"} };
        _mockUserService.GetAllAsync().Returns(users);

        var result = await _controller.GetAll();

        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(users);
    }
}
