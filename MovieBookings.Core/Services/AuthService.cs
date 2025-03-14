using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Interfaces;
using MovieBookings.Data;

namespace MovieBookings.Core.Services;

public class AuthService(ApplicationDbContext dbContext, TokenProvider tokenProvider) : IAuthService
{
    public async Task<string> Login(string email, string password)
    {
        User? user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        if(BCrypt.Net.BCrypt.Verify(password, user.Password) == false)
        {
            throw new Exception("Password incorrect");
        }

        return tokenProvider.Create(user);
    }

    public async Task<User> Register(CreateUserRequest userRequest)
    {
        if (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == userRequest.Email) != null)
        {
            throw new Exception("The email is already in use");
        }

        var user = new User
        {
            Email = userRequest.Email,
            Name = userRequest.Name,
            Password = BCrypt.Net.BCrypt.HashPassword(userRequest.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return user;
    }
}
