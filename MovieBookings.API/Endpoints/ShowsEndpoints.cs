using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Interfaces;

namespace MovieBookings.API.Endpoints;

public static class ShowsEndpoints
{
    public static void MapShowsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/shows");

        group.MapGet("/", async Task<Ok<List<ShowResponse>>> (
            [FromServices] IShowService showService) =>
        {
            var shows = await showService.GetAllAsync();
            return TypedResults.Ok(shows);
        })
        .WithOpenApi(op =>
        {
            op.Responses["200"].Description = "All the shows available with seats details.";
            return op;
        })
        .WithTags("Shows")
        .WithSummary("Get all shows")
        .WithDescription("Gets all shows available with seats details.");

        group.MapGet("/{id:int}", async Task<Results<Ok<ShowResponse>, NotFound>> (
               [FromRoute] int id,
               [FromServices] IShowService showService) =>
        {
            var show = await showService.GetByIdAsync(id);
            return show is null ? TypedResults.NotFound() : TypedResults.Ok(show);
        })
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithTags("Shows")
        .WithSummary("Get a specific show")
        .WithDescription("Gets the show with the specified ID.");
    }
}
