using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;

namespace MovieBookings.API.Endpoints;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/bookings");

        group.MapGet("/", async Task<Ok<List<BookingResponse>>> (
            [FromQuery] int userId,
            [FromServices] IBookingService bookingService) =>
        {
            var bookings = await bookingService.GetAllByUserIdAsync(userId);
            return TypedResults.Ok(bookings);
        })
        .WithTags("Bookings")
        .WithSummary("Get all bookings for a user")
        .WithDescription("Gets all bookings for a specific user.");

        group.MapPost("/", async Task<Results<Ok<BookingResponse>, ProblemHttpResult>> (
            [FromQuery] int userId,
            [FromBody] List<BookingRequest> bookings,
            [FromServices] IBookingService bookingService) =>
        {
            try
            {
                var booking = await bookingService.CreateBookingAsync(userId, bookings);
                return TypedResults.Ok(booking);
            }
            catch (SeatAlreadyBookedException ex)
            {
                return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        })
        .WithSummary("Get a specific show")
        .WithDescription("Gets the show with the specified ID.");

        group.MapDelete("/{id}", async Task<NoContent> (
            [FromRoute] int id,
            [FromServices] IBookingService bookingService) =>
        {
            await bookingService.DeleteAsync(id);
            return TypedResults.NoContent();
        })
        .WithSummary("Delete a booking")
        .WithDescription("Delete the booking with the specified ID.");
    }
}
