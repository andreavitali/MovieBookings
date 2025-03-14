using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Services;
using MovieBookings.Core;

namespace MovieBookings.API.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<TokenProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IShowService, ShowService>();
        services.AddScoped<IBookingService, BookingService>();
        return services;
    }

    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter the token returned by POST /api/auth/login in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    []
                }
            };

            o.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}
