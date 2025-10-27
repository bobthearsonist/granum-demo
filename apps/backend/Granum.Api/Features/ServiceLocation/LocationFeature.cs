namespace Granum.Api.Features.ServiceLocation;

public class LocationFeature
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public required FeatureType FeatureType { get; set; }
    public required float Measurement { get; set; }
    public required MeasurementUnit Unit { get; set; }
    public string? Description { get; set; }
}

public enum FeatureType
{
    Trees,
    Driveway,
    Edges,
    Beds,
    Acreage
}

public enum MeasurementUnit
{
    Count,
    SquareFeet,
    LinearFeet,
    Acres
}
