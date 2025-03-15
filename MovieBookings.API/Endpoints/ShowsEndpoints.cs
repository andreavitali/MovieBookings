using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Services;
using System.ComponentModel.DataAnnotations;

namespace MovieBookings.API.Endpoints;

public static class ShowsEndpoints
{
    public static void MapShowsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/shows")
            .AllowAnonymous()
            .WithTags("Shows");

        group.MapGet("/", ShowsEndpoints.GetAll)
            .WithOpenApi(op =>
            {
                op.Summary = "Get all shows";
                op.Description = "Gets all shows available with seats details.";
                op.Responses["200"].Description = "All the shows available with seats details.";
                return op;
            });

        group.MapGet("/{id:int}", ShowsEndpoints.GetById)
            .WithOpenApi(op =>
            {
                op.Summary = "Get a specific show";
                op.Description = "Gets the show with the specified Id.";
                op.Responses["200"].Description = "Show with seats details.";
                return op;
            });
    }

    private static async Task<Ok<List<ShowResponse>>> GetAll(
        [FromServices] IShowService showService)
    {
        var shows = await showService.GetAllAsync();
        return TypedResults.Ok(shows);
    }

    private static async Task<Results<Ok<ShowResponse>, NotFound>> GetById(
        [FromRoute] int id,
        [FromServices] IShowService showService)
    {
        var show = await showService.GetByIdAsync(id);
        return show is null ? TypedResults.NotFound() : TypedResults.Ok(show);
    }
}
