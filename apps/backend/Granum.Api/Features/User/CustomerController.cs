using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : Controller
{
    [HttpGet()]
    public async Task<IActionResult> GetAllCustomerAsync([FromServices] IUserService<Customer> customerService)
    {
        var customers = await customerService.GetAllAsync();
        return Ok(customers);
    }
}