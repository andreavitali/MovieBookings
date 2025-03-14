using MovieBookings.Data;
using System.ComponentModel;
namespace MovieBookings.Core;

// Auth
public record LoginRequest(string Email, string Password);
public record CreateUserRequest(string Email, string Name, string Password);
public record TokenResponse(string Token);

// Shows
public record ShowResponse(int Id, Movie Movie, DateTime StartAt, List<Seat> Seats);
public record Seat(int Id, string SeatNumber, double Price, ShowSeatStatus Status);

// Bookings
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

// Mapping Entities -> DTOs
public static class DTOMapping
{
    public static ShowResponse MapToShowDTO(this Show showEntity)
    {
        return new ShowResponse(
            showEntity.Id,
            showEntity.Movie,
            showEntity.StartAt,
            showEntity.Seats.Select(s => new Seat(s.Id, s.SeatNumber, s.Price, s.Status)).ToList()
        );
    }

    public static BookingResponse MapToBookingDTO(this Booking bookingEntity)
    {
        return new BookingResponse(
            bookingEntity.Id,
            bookingEntity.UserId,
            bookingEntity.TotalPrice,
            bookingEntity.BookedSeats.Select(s => new BookedSeat(s.Id, s.Show.Movie, s.SeatNumber, s.Show.StartAt, s.Price)).ToList()
        );
    }
}
