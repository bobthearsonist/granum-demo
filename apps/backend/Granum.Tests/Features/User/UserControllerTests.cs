using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentValidation;
using Granum.Api.Features.User;
using Microsoft.AspNetCore.JsonPatch;
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
    private IUserService<TUser> _mockUserService;
    private ILogger _mockLogger;
    private UserControllerBase<TUser> _controller;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _mockUserService = Substitute.For<IUserService<TUser>>();
        _mockLogger = Substitute.For<ILogger>();
        _controller = Substitute.ForPartsOf<UserControllerBase<TUser>>(
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

    [Test]
    [Explicit("this is telling you something... its not worth all the mocking to do this. use an integration suite for the controllers")]
    public async Task Update_WithInvalidPatch_ReturnsBadRequest()
    {
        var user = new TUser{Id = 1, Name = "Test"};
        _mockUserService.GetByIdAsync(1).Returns(user);

        var patchDoc = new JsonPatchDocument<TUser>();
        patchDoc.Replace(u => u.Name, null);

        var result = await _controller.Update(1, patchDoc);
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
