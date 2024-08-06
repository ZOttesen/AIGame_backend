namespace AIGame_backend.Services;

using Controllers;
using Microsoft.EntityFrameworkCore;
using Models;

public class UserService(UserContext context)
{
    public async Task<User> FindUserByGuidAsync(Guid userGuid)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public async Task<User> FirstOrDefaultAsync(Guid userGuid)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserGuid == userGuid);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public async Task<User> FirstOrDefaultAsync(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public async Task UpdateUser(User user, EditUserRequest request)
    {
        if (request.Email != null)
        {
            await EmailExists(request.Email);
            user.Email = request.Email;
        }

        if (request.Username != null)
        {
            await UserExists(request.Username);
            user.Username = request.Username;
        }

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;

        await SaveChangesAsync();
    }

    public async Task EmailExists(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        if (user != null)
        {
            throw new KeyNotFoundException("Email already in use");
        }
    }

    public async Task UserExists(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
        if (user != null)
        {
            throw new KeyNotFoundException("Username already in use");
        }
    }

    public async Task Add(User user)
    {
        context.Users.Add(user);
        await SaveChangesAsync();
    }

    public async Task Remove(User user)
    {
        context.Users.Remove(user);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}