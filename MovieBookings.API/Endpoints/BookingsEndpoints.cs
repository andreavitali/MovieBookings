using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Interfaces;

namespace MovieBookings.API.Endpoints;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/bookings");

        //group.MapGet("/", async Task<Ok<List<ShowResponse>>> (
        //    [FromServices] IShowService showService) =>
        //{
        //    var shows = await showService.GetAllAsync();
        //    return TypedResults.Ok(shows);
        //})
        //.WithSummary("Get all shows")
        //.WithDescription("Gets all shows available with seats details.");

        //group.MapGet("/{id}", async Task<Results<Ok<ShowResponse>, NotFound>> (
        //       int id,
        //       [FromServices] IShowService showService) =>
        //{
        //    var show = await showService.GetByIdAsync(id);
        //    return show is null ? TypedResults.NotFound() : TypedResults.Ok(show);
        //})
        //.ProducesProblem(StatusCodes.Status404NotFound)
        //.WithSummary("Get a specific show")
        //.WithDescription("Gets the show with the specified ID.");
    }
}
