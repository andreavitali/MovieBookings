﻿using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Models;
using MovieBookings.Data;

namespace MovieBookings.Core.Services;

public class AuthService(ApplicationDbContext dbContext, TokenProvider tokenProvider) : IAuthService
{
    public async Task<TokenResponse> Login(string email, string password)
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

        var token = tokenProvider.Create(user);
        return new TokenResponse(token);
    }

    public async Task<User> Register(CreateUserRequest userRequest)
    {
        if (await dbContext.Users.SingleOrDefaultAsync(u => u.Email == userRequest.Email) != null)
        {
            throw new EmailAlreadyInUseException(userRequest.Email);
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
