using Granum.Api.Features.Service;
using Granum.Api.Features.User;

namespace Granum.Api.Features.Estimate;

public class Estimate
{
    public Estimate() { }
    public Estimate(int id, float total)
    {
        Id = id;
        Total = total;
    }
    public int Id { get; set; }
    public float Total { get; set; }
    public required Customer Customer { get; set; }
    public required Contractor Contractor { get; set; }
    public DateTime CreatedDate { get; set; }
    public required LineItem[] LineItems { get; set; }
}
