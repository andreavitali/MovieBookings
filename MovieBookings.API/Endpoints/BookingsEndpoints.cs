using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MovieBookings.Core;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;
using System.IdentityModel;
using System.Security.Claims;

namespace MovieBookings.API.Endpoints;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/bookings")
            .WithTags("Bookings")
            .WithOpenApi();

        group.MapGet("/", async Task<Ok<List<BookingResponse>>> (
            HttpContext context,
            [FromServices] IBookingService bookingService) =>
        {
            var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var bookings = await bookingService.GetAllByUserIdAsync(userId);
            return TypedResults.Ok(bookings);
        })
        .RequireAuthorization()
        .WithSummary("Get all bookings for a user")
        .WithDescription("Gets all bookings for a specific user.");

        group.MapPost("/", async Task<Results<Ok<BookingResponse>, ProblemHttpResult>> (
            HttpContext context,
            [FromBody] List<BookingRequest> bookings,
            [FromServices] IBookingService bookingService) =>
        {
            // Validation (should be done with a 3rd party library!)
            if (bookings.Count == 0)
            {
                return TypedResults.Problem("Add at least one BookingRequest", statusCode: StatusCodes.Status400BadRequest);
            }

            if(bookings.Any(br => br.SeatId is null && br.ShowId is null))
            {
                return TypedResults.Problem("At least one between seatId and showId should be defined", statusCode: StatusCodes.Status400BadRequest);
            }

            var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            try
            {
                var booking = await bookingService.CreateBookingAsync(1, bookings);
                return TypedResults.Ok(booking);
            }
            catch (SeatAlreadyBookedException ex)
            {
                return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        })
        .RequireAuthorization()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithOpenApi(op =>
        {
            op.Responses["200"].Description = "Created booking with all the seats reserved";
            op.Responses["400"].Description = "Model validation error";
            op.Responses["409"].Description = "One ore more seats are already booked";
            return op;
        })
        .WithSummary("Create a booking for a user")
        .WithDescription("""
            Book one or multiple seats in one or multiple shows.  
            The request body should contain a list of BookingRequest objects, each one containing a showId (seat selected random if available) or a seatId (specific seat of a specific show).
            """);

        group.MapDelete("/{id}", async Task<Results<ForbidHttpResult,NoContent>> (
            HttpContext context,
            [FromRoute] int id,
            [FromServices] IBookingService bookingService) =>
        {
            var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var booking = await bookingService.GetByIdAsync(id);
            if(booking?.UserId != userId)
            {
                return TypedResults.Forbid();
            }

            await bookingService.DeleteAsync(id);
            return TypedResults.NoContent();
        })
        .RequireAuthorization()
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Delete a booking")
        .WithDescription("Delete the booking with the specified ID.");
    }
}
