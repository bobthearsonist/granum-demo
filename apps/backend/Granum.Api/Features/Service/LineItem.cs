using System.ComponentModel.DataAnnotations;

namespace Granum.Api.Features.Service;

public class LineItem
{
    public int Id { get; set; }
    public float Cost { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    public required Estimate.Estimate Estimate { get; set; }
    public required ServiceType ServiceType { get; set; }
    public required Frequency Frequency { get; set; }
}

public class ServiceType
{
    public int Id { get; set; }
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(1024)]
    public required string Description { get; set; }
    public Category Category { get; set; }
    //TODO strategy pattern pricing formula
}

public enum Category
{
    Lawn, Tree
}