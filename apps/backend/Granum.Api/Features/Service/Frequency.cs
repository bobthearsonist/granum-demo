namespace Granum.Api.Features.Service;

public class Frequency
{
    public int Id { get; set; }
    public FrequencyType Type { get; set; }
    public int Value { get; set; }
}

public enum FrequencyType
{
    None=0,
    Daily,
    Weekly,
    Monthly,
    Seasonal,
    Yearly
}