namespace Granum.Api.Features.User;

public class Customer : User
{
    public Customer() { }
    public Customer(string name) { Name = name; }
}

public class CustomerValidator : UserValidator<Customer>;
