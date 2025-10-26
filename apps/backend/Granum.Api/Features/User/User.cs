using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Granum.Api.Features.User;

public abstract class User
{
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }
}

// TODO at this point I think having both Data Annotations and FluentValidation is redundant and overcomplicated. But I didnt think about that... and I like having this her eas an example of how the custom annotations work with FluentValidation... so im going to leave it for now i think adn rip it out later. probably when its hard to write tests.
public class UserValidator : AbstractValidator<User>
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