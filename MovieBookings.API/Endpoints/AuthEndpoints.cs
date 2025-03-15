using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using MiniValidation;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Models;
using MovieBookings.Data;
using System.Text.Json;
using LoginRequest = MovieBookings.Core.Models.LoginRequest;

namespace MovieBookings.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/auth")
            .AllowAnonymous()
            .WithTags("Auth");

        group.MapPost("/login", AuthEndpoints.Login)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithOpenApi(op =>
            {
                op.Summary = "Login user";
                op.Description = "Login using email and password";
                op.RequestBody.Content["application/json"].Example = new OpenApiObject
                {
                    ["email"] = new OpenApiString("andrea@email.com"),
                    ["password"] = new OpenApiString("password")
                };
                var response200 = op.Responses["200"];
                response200.Content["application/json"].Example = new OpenApiString(
                    JsonSerializer.Serialize(new { token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6IkFuZHJlYSIsImVtYWlsIjoiYW5kcmVhQGVtYWlsLmNvbSIsImF1ZCI6Ik1vdmllQm9va2luZ3MuQVBJIiwiaXNzIjoiTW92aWVCb29raW5ncyIsImV4cCI6MTc0MTk1MzE1MywiaWF0IjoxNzQxOTUyNTUzLCJuYmYiOjE3NDE5NTI1NTN9.cj3VtbVAdhykwimqRv7QcRueU1h2jZfJe88h4TAsS3E" }));
                response200.Description = "JWT Token";
                return op;
            });

        group.MapPost("/register", AuthEndpoints.Register)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Register a user";
                op.Description = "Add a new user in the system";
                op.Responses["200"].Description = "Created user";
                op.Responses["409"].Description = "The email is already in use";
                return op;
            });
    }

    public static async Task<Results<Ok<TokenResponse>, ValidationProblem, ProblemHttpResult>> Login(
        [FromBody] LoginRequest loginRequest,
        [FromServices] IAuthService authService)
    {
        try
        {
            if(!MiniValidator.TryValidate(loginRequest, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }
            var tokenResponse = await authService.Login(loginRequest.Email, loginRequest.Password);
            return TypedResults.Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized);
        }
    }

    private static async Task<Results<Ok<User>, ValidationProblem, ProblemHttpResult>> Register(
        [FromBody] CreateUserRequest userRequest,
        [FromServices] IAuthService authService)
    {
        try
        {
            if (!MiniValidator.TryValidate(userRequest, out var errors))
            {
                return TypedResults.ValidationProblem(errors);
            }
            var user = await authService.Register(userRequest);
            return TypedResults.Ok(user);
        }
        catch (EmailAlreadyInUseException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }           
    }
}
