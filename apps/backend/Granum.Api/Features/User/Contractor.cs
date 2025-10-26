using FluentValidation;

namespace Granum.Api.Features.User;

public class Contractor : User
{
    public Contractor() { }
    public Contractor(string name) { Name = name; }
}

public class ContractorValidator : UserValidator<Contractor>;