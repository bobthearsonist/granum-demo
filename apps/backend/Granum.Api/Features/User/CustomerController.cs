using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : Controller
{
    [HttpGet()]
    public async Task<IActionResult> GetAllCustomerAsync([FromServices] IUserRepository<Customer> customerRepository)
    {
        //TODO this should be an injected service
        var customers = await customerRepository.GetAllAsync();
        return Ok(customers);
    }
}