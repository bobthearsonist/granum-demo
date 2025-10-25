using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

public abstract class UserControllerBase<TUser> : Controller where TUser : User
{
    protected readonly IUserService<TUser> UserService;

    protected UserControllerBase(IUserService<TUser> userService)
    {
        UserService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await UserService.GetAllAsync());
}

[Route("api/[controller]")]
public class CustomersController(IUserService<Customer> service)
    : UserControllerBase<Customer>(service) { }

[Route("api/[controller]")]
public class ContractorsController(IUserService<Contractor> service)
    : UserControllerBase<Contractor>(service) { }