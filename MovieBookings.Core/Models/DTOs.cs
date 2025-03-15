using MovieBookings.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace MovieBookings.Core.Models;

// Auth
public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password);

public record CreateUserRequest(
    [Required][EmailAddress] string Email, 
    [Required] string Name,
    [Required][MinLength(8)] string Password);

public record TokenResponse(string Token);

// Shows
public record ShowResponse(int Id, Movie Movie, DateTime StartAt, List<Seat> Seats);
public record Seat(int Id, string SeatNumber, double Price, ShowSeatStatus Status);

// Bookings
public record UserBookingsRequest(
    [property: Description("One or more booking requests")] List<BookingRequest> BookingRequests) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BookingRequests.Count == 0)
        {
            yield return new ValidationResult("At least one booking request is required", [nameof(BookingRequests)]);
        }

        if (BookingRequests.Any(br => br.SeatId is null && br.ShowId is null))
        {
            yield return new ValidationResult("At least one between seatId and showId should be defined", [nameof(BookingRequests)]);
        }
    }
}

public record BookingRequest(
    [property: Description("Id of a show for which to get a random seat")] int? ShowId,
    [property: Description("Id of a specific seat for a show")] int? SeatId);

public record BookingResponse(
    [property: Description("Unique Id of the booking")] int Id,
    [property: Description("Id of the user that completed the booking")] int UserId,
    [property: Description("Total booking price (sum of the price of each seat)")] double TotalPrice,
    [property: Description("Details of each seat booked")] List<BookedSeat> BookedSeats);

public record BookedSeat(
    [property: Description("Unique Id of the seat for that show")] int Id,
    [property: Description("Details of the movie")] Movie Movie,
    [property: Description("Seat identifier in the theatre")] string SeatNumber,
    [property: Description("Show starting date and time")] DateTime StartAt, 
    [property: Description("Price of the seat for the show")] double Price);
