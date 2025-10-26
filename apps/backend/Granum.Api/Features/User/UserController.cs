using Granum.Api.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

public abstract class UserControllerBase<TUser>(
    IUserService<TUser> userService,
    ILogger logger) : Controller
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
        => CreatedAtAction(nameof(Get), await userService.CreateAsync(user));

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JsonPatchDocument<TUser> patchDoc)
    {
        var user = await userService.GetByIdAsync(id);

        patchDoc.ApplyTo(user, ModelState);
        if (!ModelState.IsValid) throw new ArgumentException("Invalid patch document.", nameof(patchDoc));

        await userService.UpdateAsync(user);
        return NoContent();
    }
}

[Route("api/[controller]")]
public class CustomersController(IUserService<Customer> service, ILogger<CustomersController> logger)
    : UserControllerBase<Customer>(service, logger);

[Route("api/[controller]")]
public class CustomerController(IUserService<Customer> service, ILogger<CustomersController> logger)
    : UserControllerBase<Customer>(service, logger);

[Route("api/[controller]")]
public class ContractorsController(IUserService<Contractor> service, ILogger<CustomersController> logger)
    : UserControllerBase<Contractor>(service, logger);

[Route("api/[controller]")]
public class ContractorController(IUserService<Contractor> service, ILogger<CustomersController> logger)
    : UserControllerBase<Contractor>(service, logger);