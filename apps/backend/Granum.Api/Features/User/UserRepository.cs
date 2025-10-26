using Granum.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Granum.Api.Features.User;

public interface IUserRepository<TUser> : IRepository<TUser> where TUser : User{
    Task<IEnumerable<TUser>> GetByNameAsync(string name);
    Task<TUser?> FindByExactNameAsync(string name);
}

// TODO sort functions by name DESC
public class UserRepository<TUser>(IAppDbContext context) : IUserRepository<TUser> where TUser : User
{
    public Task<TUser?> GetByIdAsync(int id) => context.Set<TUser>().FindAsync(id).AsTask();

    public async Task<IEnumerable<TUser>> GetAllAsync() => await context.Set<TUser>().ToListAsync();

    public async Task<TUser> AddAsync(TUser entity)
    {
        var modified = context.Add(entity);
        await context.SaveChangesAsync();
        return modified.Entity;
    }

    public async Task<TUser?> UpdateAsync(TUser entity)
    {
        context.Update(entity);
        await context.SaveChangesAsync();
        return entity;
    }
    public Task DeleteAsync(int id) => throw new NotImplementedException();
    public Task<bool> ExistsAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<TUser>> GetByNameAsync(string name) => throw new NotImplementedException();
    public Task<TUser?> FindByExactNameAsync(string name) => throw new NotImplementedException();
}
