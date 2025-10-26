using System.ComponentModel.DataAnnotations;
using FluentValidation;
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

public static class UserExtensions
{
    public static async Task<(bool IsValid, IActionResult? ErrorResult)> ValidateAsync<TUser>(
        this TUser user, IServiceProvider services) where TUser : User
    {
        // Data Annotations
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(user, new ValidationContext(user), results, true))
            return (false, new BadRequestObjectResult(new ValidationProblemDetails(
                results.ToDictionary(e => e.MemberNames.First(), e => new[] { e.ErrorMessage! }))));

        // FluentValidation
        if (services.GetService(typeof(IValidator<TUser>)) is not IValidator validator)
            return (true, null);

        var result = await validator.ValidateAsync(new ValidationContext<object>(user));
        return result.IsValid
            ? (true, null)
            : (false, new UnprocessableEntityObjectResult(
                new ValidationProblemDetails(result.ToDictionary()) { Status = 422 }));
    }
}
