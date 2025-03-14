using MovieBookings.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookings.Core.Interfaces;

public interface IAuthService
{
    Task<string> Login(string email, string password);
    Task<User> Register(CreateUserRequest userRequest);
}
