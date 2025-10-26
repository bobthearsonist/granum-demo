using FluentAssertions;
using Granum.Api.Features.ServiceLocation;
using NSubstitute;

namespace Granum.Tests.Features.ServiceLocation;

[TestFixture]
public class ServiceLocationServiceTests
{
    private IServiceLocationRepository _mockRepository;
    private ServiceLocationService _service;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _mockRepository = Substitute.For<IServiceLocationRepository>();
        _service = new ServiceLocationService(_mockRepository);
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetAllAsync_CallsRepository() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task CreateAsync_CallsRepository() => Task.CompletedTask;

    [Test]
    public async Task GetByIdAsync_WhenLocationNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepository.GetByIdAsync(0).Returns((Api.Features.ServiceLocation.ServiceLocation?)null!);

        await _service.Invoking(s => s.GetByIdAsync(0))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("ServiceLocation with ID 0 not found.");
    }

    [Test]
    public async Task UpdateAsync_WhenLocationNotFound_ThrowsKeyNotFoundException()
    {
        var location = new Api.Features.ServiceLocation.ServiceLocation { Id = 1, CustomerId = 1, Name = "Test", Address = "123 Main St" };
        _mockRepository.UpdateAsync(location).Returns((Api.Features.ServiceLocation.ServiceLocation?)null!);

        await _service.Invoking(s => s.UpdateAsync(location))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("ServiceLocation with ID 1 not found.");
    }
}
