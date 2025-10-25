using Microsoft.AspNetCore.Mvc;

namespace Granum.Api.Features.User;

public interface IUserService<TUser>
{
    Task<IEnumerable<TUser>> GetAllAsync();
}

public class UserService<TUser>([FromServices] IUserRepository<TUser> userRepository) : IUserService<TUser> where TUser : User
{
    public async Task<IEnumerable<TUser>> GetAllAsync()
    {
        return await userRepository.GetAllAsync();
    }
}