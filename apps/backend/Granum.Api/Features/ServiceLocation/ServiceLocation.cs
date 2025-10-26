using System.ComponentModel.DataAnnotations;

namespace Granum.Api.Features.ServiceLocation;

public class ServiceLocation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer ID is required")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public required string Address { get; set; }
}
