namespace Granum.Api.Features.User;

public abstract class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
}