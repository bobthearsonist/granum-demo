namespace Granum.Api.Features.Location;

public class Location
{
    public Location() { }

    public Location(string address, string name)
    {
        Address = address;
        Name = name;
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; } // TODO this is intentionally simplified
}