using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using FluentValidation;
using Granum.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Granum.Tests.Infrastructure.Validation;

[TestFixture]
public class EntityValidationExtensionsTests
{
    // Test entity with Data Annotations
    private class TestEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? Name { get; set; }

        [System.ComponentModel.DataAnnotations.Range(1, 100, ErrorMessage = "Age must be between 1 and 100")]
        public int Age { get; set; }
    }

    // FluentValidation validator for TestEntity
    private class TestEntityValidator : AbstractValidator<TestEntity>
    {
        public TestEntityValidator()
        {
            RuleFor(x => x.Name).Custom((name, ctx) =>
            {
                if (name?.Contains("forbidden", StringComparison.OrdinalIgnoreCase) == true)
                    ctx.AddFailure("Name", "Name cannot contain 'forbidden'");
            });
        }
    }

    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void Setup()
    {
        // Default service provider with no FluentValidation registered
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
    }

    [Test]
    public async Task ValidateAsync_WithValidEntity_ReturnsTrue()
    {
        // Arrange
        var entity = new TestEntity { Name = "John Doe", Age = 30 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeTrue();
        errorResult.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WithInvalidDataAnnotation_Returns400BadRequest()
    {
        // Arrange - missing required Name
        var entity = new TestEntity { Name = null, Age = 30 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<BadRequestObjectResult>();
        
        var badRequestResult = errorResult as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ValidationProblemDetails>();
        
        var problemDetails = badRequestResult.Value as ValidationProblemDetails;
        problemDetails!.Errors.Should().ContainKey("Name");
        problemDetails.Errors["Name"].Should().Contain("Name is required");
    }

    [Test]
    public async Task ValidateAsync_WithInvalidDataAnnotationStringLength_Returns400BadRequest()
    {
        // Arrange - Name exceeds 50 characters
        var entity = new TestEntity 
        { 
            Name = "This is a very long name that exceeds fifty characters limit", 
            Age = 30 
        };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<BadRequestObjectResult>();
        
        var badRequestResult = errorResult as BadRequestObjectResult;
        var problemDetails = badRequestResult!.Value as ValidationProblemDetails;
        problemDetails!.Errors.Should().ContainKey("Name");
        problemDetails.Errors["Name"].Should().Contain("Name cannot exceed 50 characters");
    }

    [Test]
    public async Task ValidateAsync_WithInvalidDataAnnotationRange_Returns400BadRequest()
    {
        // Arrange - Age out of range
        var entity = new TestEntity { Name = "John Doe", Age = 150 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<BadRequestObjectResult>();
        
        var badRequestResult = errorResult as BadRequestObjectResult;
        var problemDetails = badRequestResult!.Value as ValidationProblemDetails;
        problemDetails!.Errors.Should().ContainKey("Age");
        problemDetails.Errors["Age"].Should().Contain("Age must be between 1 and 100");
    }

    [Test]
    public async Task ValidateAsync_WithFluentValidationRegistered_AndValidEntity_ReturnsTrue()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IValidator<TestEntity>, TestEntityValidator>();
        _serviceProvider = services.BuildServiceProvider();

        var entity = new TestEntity { Name = "John Doe", Age = 30 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeTrue();
        errorResult.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WithFluentValidationRegistered_AndInvalidEntity_Returns422UnprocessableEntity()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IValidator<TestEntity>, TestEntityValidator>();
        _serviceProvider = services.BuildServiceProvider();

        var entity = new TestEntity { Name = "forbidden word", Age = 30 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<UnprocessableEntityObjectResult>();
        
        var unprocessableResult = errorResult as UnprocessableEntityObjectResult;
        unprocessableResult!.Value.Should().BeOfType<ValidationProblemDetails>();
        
        var problemDetails = unprocessableResult.Value as ValidationProblemDetails;
        problemDetails!.Status.Should().Be(422);
        problemDetails.Errors.Should().ContainKey("Name");
        problemDetails.Errors["Name"].Should().Contain("Name cannot contain 'forbidden'");
    }

    [Test]
    public async Task ValidateAsync_WithBothInvalidDataAnnotationAndFluentValidation_ReturnsDataAnnotationError()
    {
        // Arrange - Data Annotations should fail first
        var services = new ServiceCollection();
        services.AddScoped<IValidator<TestEntity>, TestEntityValidator>();
        _serviceProvider = services.BuildServiceProvider();

        var entity = new TestEntity { Name = null, Age = 30 }; // Missing required Name

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert - Data Annotations validation should fail first with 400
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<BadRequestObjectResult>(); // 400, not 422
        
        var badRequestResult = errorResult as BadRequestObjectResult;
        var problemDetails = badRequestResult!.Value as ValidationProblemDetails;
        problemDetails!.Errors.Should().ContainKey("Name");
        problemDetails.Errors["Name"].Should().Contain("Name is required");
    }

    [Test]
    public async Task ValidateAsync_WithoutFluentValidationRegistered_OnlyValidatesDataAnnotations()
    {
        // Arrange - No FluentValidation registered
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();

        // Entity that would fail FluentValidation but passes Data Annotations
        var entity = new TestEntity { Name = "forbidden word", Age = 30 };

        // Act
        var (isValid, errorResult) = await entity.ValidateAsync(_serviceProvider);

        // Assert - Should pass because FluentValidation is not registered
        isValid.Should().BeTrue();
        errorResult.Should().BeNull();
    }

    [Test]
    public async Task ValidateAsync_WithGenericEntityType_WorksCorrectly()
    {
        // Arrange - Test with a different entity type to prove genericity
        var differentEntity = new AnotherTestEntity { Value = "test" };

        // Act
        var (isValid, errorResult) = await differentEntity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeTrue();
        errorResult.Should().BeNull();
    }

    // Another test entity to prove the generic nature
    private class AnotherTestEntity
    {
        [Required]
        public string? Value { get; set; }
    }

    [Test]
    public async Task ValidateAsync_WithInvalidGenericEntityType_Returns400BadRequest()
    {
        // Arrange
        var differentEntity = new AnotherTestEntity { Value = null };

        // Act
        var (isValid, errorResult) = await differentEntity.ValidateAsync(_serviceProvider);

        // Assert
        isValid.Should().BeFalse();
        errorResult.Should().NotBeNull();
        errorResult.Should().BeOfType<BadRequestObjectResult>();
    }
}
