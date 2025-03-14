using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using MovieBookings.Core;
using MovieBookings.Core.Interfaces;
using MovieBookings.Data;

namespace MovieBookings.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/auth")
            .AllowAnonymous()
            .WithTags("Auth")
            .WithOpenApi();

        group.MapPost("/login", async Task<Results<ContentHttpResult, ProblemHttpResult>> (
            [FromBody] LoginRequest loginRequest,
            [FromServices] IAuthService authService) =>
        {
            try
            {
                var token = await authService.Login(loginRequest.Email, loginRequest.Password);
                return TypedResults.Text(token);
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized);
            }
        })
        .Produces<string>(contentType: "text/plain")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .WithSummary("Login user")
        .WithDescription("Login using email and password")
        .WithOpenApi(op =>
        {
            var response200 = op.Responses["200"];
            response200.Content["text/plain"].Example = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwibmFtZSI6IkFuZHJlYSIsImVtYWlsIjoiYW5kcmVhQGVtYWlsLmNvbSIsImF1ZCI6Ik1vdmllQm9va2luZ3MuQVBJIiwiaXNzIjoiTW92aWVCb29raW5ncyIsImV4cCI6MTc0MTk1MzE1MywiaWF0IjoxNzQxOTUyNTUzLCJuYmYiOjE3NDE5NTI1NTN9.cj3VtbVAdhykwimqRv7QcRueU1h2jZfJe88h4TAsS3E");
            response200.Description = "JWT Token";
            return op;
        });

        group.MapPost("/register", async Task<Results<Ok<User>, Conflict<string>>> (
        [FromBody] CreateUserRequest userRequest,
        [FromServices] IAuthService authService) =>
        {
            try
            {
                var user = await authService.Register(userRequest);
                return TypedResults.Ok(user);
            }
            catch (Exception ex)
            {
                return TypedResults.Conflict(ex.Message);
            }           
        })
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithOpenApi(op =>
        {
            op.Responses["200"].Description = "Created user";
            op.Responses["409"].Description = "The email is already in use";
            return op;
        })
        .WithSummary("Register a user")
        .WithDescription("Add a new user in the system");
    }
}
