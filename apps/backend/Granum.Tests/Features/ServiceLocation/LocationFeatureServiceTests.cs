using FluentAssertions;
using Granum.Api.Features.ServiceLocation;
using NSubstitute;

namespace Granum.Tests.Features.ServiceLocation;

[TestFixture]
public class LocationFeatureServiceTests
{
    private ILocationFeatureRepository _mockRepository;
    private LocationFeatureService _service;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _mockRepository = Substitute.For<ILocationFeatureRepository>();
        _service = new LocationFeatureService(_mockRepository);
    }

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task GetByLocationIdAsync_CallsRepository() => Task.CompletedTask;

    [Ignore("Pure delegation - no logic to test")]
    [Test]
    public Task CreateAsync_CallsRepository() => Task.CompletedTask;

    [Test]
    public async Task GetByIdAsync_WhenFeatureNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepository.GetByIdAsync(0).Returns((LocationFeature?)null!);

        await _service.Invoking(s => s.GetByIdAsync(0))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("LocationFeature with ID 0 not found.");
    }

    [Test]
    public async Task UpdateAsync_WhenFeatureNotFound_ThrowsKeyNotFoundException()
    {
        var feature = new LocationFeature 
        { 
            Id = 1, 
            LocationId = 1, 
            FeatureType = FeatureType.Trees,
            Measurement = 5.0f,
            Unit = MeasurementUnit.Count
        };
        _mockRepository.UpdateAsync(feature).Returns((LocationFeature?)null!);

        await _service.Invoking(s => s.UpdateAsync(feature))
            .Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("LocationFeature with ID 1 not found.");
    }
}
