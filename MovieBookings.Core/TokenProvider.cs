using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MovieBookings.Data;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace MovieBookings.Core;

public sealed class TokenProvider(IConfiguration configuration)
{
    public string Create(User user)
    {
        string secretKey = configuration["Jwt:SecretKey"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = signingCredentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(securityTokenDescriptor);
        return token;
    }
}
