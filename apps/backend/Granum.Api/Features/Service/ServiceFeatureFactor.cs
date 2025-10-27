namespace Granum.Api.Features.Service;

public class ServiceFeatureFactor
{
    public int Id { get; set; }
    public int ServiceTypeId { get; set; }
    public required string FeatureType { get; set; }
    public required float Multiplier { get; set; }
    public string? CalculationNotes { get; set; }
}
