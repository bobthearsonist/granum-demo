using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

[ApiController]
public abstract class UserControllerBase<TUser>(
    IUserService<TUser> userService) : Controller
    where TUser : User
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
        => Ok(await userService.GetByIdAsync(id));

    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create(TUser user)
    {
        var createdUser = await userService.CreateAsync(user);
        return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JsonPatchDocument<TUser> patchDoc)
    {
        var user = await userService.GetByIdAsync(id);

        patchDoc.ApplyTo(user, ModelState);
        if (!ModelState.IsValid) BadRequest(patchDoc);

        var (isValid, errorResult) = await user.ValidateAsync(HttpContext.RequestServices);
        if (!isValid) return errorResult!;

        await userService.UpdateAsync(user);
        return NoContent();
    }
}

[Route("api/[controller]")]
public class CustomersController(IUserService<Customer> service, ILogger<CustomersController> logger)
    : UserControllerBase<Customer>(service);

[Route("api/[controller]")]
public class CustomerController(IUserService<Customer> service, ILogger<CustomersController> logger)
    : UserControllerBase<Customer>(service);

[Route("api/[controller]")]
public class ContractorsController(IUserService<Contractor> service, ILogger<CustomersController> logger)
    : UserControllerBase<Contractor>(service);

[Route("api/[controller]")]
public class ContractorController(IUserService<Contractor> service, ILogger<CustomersController> logger)
    : UserControllerBase<Contractor>(service);
