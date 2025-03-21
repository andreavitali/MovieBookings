﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;
using MovieBookings.Core.Models;
using System.Security.Claims;

namespace MovieBookings.API.Endpoints;

public static class BookingsEndpoints
{
    public static void MapBookingsEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/bookings")
            .WithTags("Bookings");

        group.MapGet("/", BookingsEndpoints.GetAllByUser)
            .RequireAuthorization()
            .WithSummary("Get all bookings for a user")
            .WithDescription("Gets all bookings for a specific user.");

        group.MapPost("/", BookingsEndpoints.CreateBooking)
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Create a booking for a user";
                op.Description = """
                Book one or multiple seats in one or multiple shows for the authenticated user.  
                The request body should contain a list of BookingRequest objects, each one containing a showId (seat selected random if available) or a seatId (specific seat of a specific show).
                """;
                op.Responses["200"].Description = "Created booking with all the seats reserved";
                op.Responses["400"].Description = "Model validation error";
                op.Responses["409"].Description = "One ore more seats are already booked";
                return op;
            });

        group.MapDelete("/{id:int}", BookingsEndpoints.DeleteById)
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .WithSummary("Delete a booking")
            .WithDescription("Delete the booking with the specified ID.");
    }

    private static async Task<Ok<List<BookingResponse>>> GetAllByUser(
        HttpContext context,
        [FromServices] IBookingService bookingService)
    {
        var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        var bookings = await bookingService.GetAllByUserIdAsync(userId);
        return TypedResults.Ok(bookings);
    }

    private static async Task<Results<Ok<BookingResponse>, ValidationProblem, ProblemHttpResult>> CreateBooking(
        HttpContext context,
        [FromBody] UserBookingsRequest bookings,
        [FromServices]
        IBookingService bookingService)
    {
        if (!MiniValidator.TryValidate(bookings, out var errors))
        {
            return TypedResults.ValidationProblem(errors);
        }

        var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

        try
        {
            var booking = await bookingService.CreateBookingAsync(1, bookings.BookingRequests);
            return TypedResults.Ok(booking);
        }
        catch (SeatAlreadyBookedException ex)
        {
            return TypedResults.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }

    private static async Task<Results<NoContent, ForbidHttpResult>> DeleteById(
        HttpContext context,
        [FromRoute] int id,
        [FromServices] IBookingService bookingService)
    {
        var userId = int.Parse(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

        var booking = await bookingService.GetByIdAsync(id);
        if (booking?.UserId != userId)
        {
            return TypedResults.Forbid();
        }

        await bookingService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
