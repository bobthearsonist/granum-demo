using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Granum.Api.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

public abstract class User
{
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
}

// TODO at this point I think having both Data Annotations and FluentValidation is redundant and overcomplicated. But I didnt think about that... and I like having this her eas an example of how the custom annotations work with FluentValidation... so im going to leave it for now i think adn rip it out later. probably when its hard to write tests.
public class UserValidator<TUser> : AbstractValidator<TUser> where TUser : User
{
    public UserValidator()
    {
        // FluentValidation rules override/supplement data annotations
        RuleFor(x => x.Name).Custom((name, ctx) => {
            if (name?.Contains("admin", StringComparison.OrdinalIgnoreCase) == true)
                ctx.AddFailure("Name cannot contain 'admin'");
        });
    }
}

/// <summary>
/// Extension methods for User entity validation.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Validates a User entity using Data Annotations and FluentValidation (if registered).
    /// This method delegates to the generic EntityValidationExtensions.ValidateAsync method.
    /// </summary>
    /// <typeparam name="TUser">The type of user to validate. Must inherit from User.</typeparam>
    /// <param name="user">The user instance to validate.</param>
    /// <param name="services">The service provider used to resolve FluentValidation validators.</param>
    /// <returns>
    /// A tuple containing:
    /// - IsValid: true if validation passes, false otherwise
    /// - ErrorResult: An IActionResult with validation errors, or null if validation passes
    /// </returns>
    /// <remarks>
    /// This method maintains backward compatibility while delegating to the generic validation infrastructure.
    /// See <see cref="EntityValidationExtensions.ValidateAsync{TEntity}"/> for detailed validation behavior.
    /// </remarks>
    public static async Task<(bool IsValid, IActionResult? ErrorResult)> ValidateAsync<TUser>(
        this TUser user, IServiceProvider services) where TUser : User
    {
        return await EntityValidationExtensions.ValidateAsync(user, services);
    }
}
