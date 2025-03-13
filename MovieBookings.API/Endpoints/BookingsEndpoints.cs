using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;

namespace MovieBookings.API.Endpoints;

public static class BookingsEndpoints
{
    private static string AT_LEAST_ONE_BOOKING_REQUEST_ERROR = "Add at least one BookingRequest";
    private static string NO_IDS_IN_BOOKING_REQUEST_ERROR = "At least one between seatId and showId should be not null";

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
            // Validation
            if(bookings.Count == 0)
            {
                return TypedResults.Problem("Add at least one BookingRequest", statusCode: StatusCodes.Status400BadRequest);
            }

            if(bookings.Any(br => br.SeatId is null && br.ShowId is null))
            {
                return TypedResults.Problem("At least one between seatId and showId should be defined", statusCode: StatusCodes.Status400BadRequest);
            }

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
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithTags("Bookings")
        .WithSummary("Create a booking for a user")
        .WithDescription("""
            Book one or multiple seats in one or multiple shows.  
            The request body should contain a list of BookingRequest objects, each one containing a showId (seat selected random if available) or a seatId (specific seat of a specific show).
            """);

        group.MapDelete("/{id}", async Task<NoContent> (
            [FromRoute] int id,
            [FromServices] IBookingService bookingService) =>
        {
            await bookingService.DeleteAsync(id);
            return TypedResults.NoContent();
        })
        .WithTags("Bookings")
        .WithSummary("Delete a booking")
        .WithDescription("Delete the booking with the specified ID.");
    }
}
