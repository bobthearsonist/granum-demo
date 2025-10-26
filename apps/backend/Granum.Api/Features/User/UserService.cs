using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Granum.Api.Features.User;

public interface IUserService<TUser> where TUser : class
{
    Task<IEnumerable<TUser>> GetAllAsync();
    Task<TUser> GetByIdAsync(int id);
    // TODO nullable response object?
    Task<TUser> CreateAsync(TUser user);
    Task<TUser?> UpdateAsync(TUser user);
}

public class UserService<TUser>([FromServices] IUserRepository<TUser> userRepository) : IUserService<TUser> where TUser : User
{
    private const string UserNotFoundFormat = "User with ID {0} not found.";
    public async Task<IEnumerable<TUser>> GetAllAsync() => await userRepository.GetAllAsync();

    public async Task<TUser> GetByIdAsync(int id) => await userRepository.GetByIdAsync(id) ?? UserNotFoundException(id);

    public async Task<TUser> CreateAsync(TUser user) => await userRepository.AddAsync(user);

    public async Task<TUser?> UpdateAsync(TUser user)
    {
        return await userRepository.UpdateAsync(user) ?? UserNotFoundException(user.Id);
    }

    private static TUser UserNotFoundException(int id)
    {
        throw new KeyNotFoundException(string.Format(UserNotFoundFormat, id));
    }
}