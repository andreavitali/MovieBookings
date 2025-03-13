using MovieBookings.Data;
using System.ComponentModel.DataAnnotations;

namespace MovieBookings.Core;

public record Seat(int Id, string SeatNumber, double Price, ShowSeatStatus Status);
public record BookedSeat(int Id, Movie Movie, string SeatNumber, DateTime StartAt, double Price);

public record ShowResponse(int Id, Movie Movie, DateTime StartAt, List<Seat> Seats);
public record BookingResponse(int Id, int UserId, double TotalPrice, List<BookedSeat> BookedSeats);
public record BookingRequest(int? ShowId, int? SeatId);

public static class DTOMapping
{
    public static ShowResponse MapToShowDTO(this Show showEntity)
    {
        return new ShowResponse(
            showEntity.Id,
            showEntity.Movie,
            showEntity.StartAt,
            showEntity.Seats
                .Select(s => new Seat(s.Id, s.SeatNumber, s.Price, s.Status))
                .ToList()
        );
    }

    public static BookingResponse MapToBookingDTO(this Booking bookingEntity)
    {
        return new BookingResponse(
            bookingEntity.Id,
            bookingEntity.UserId,
            bookingEntity.TotalPrice,
            bookingEntity.BookedSeats
                .Select(s => new BookedSeat(s.Id, s.Show.Movie, s.SeatNumber, s.Show.StartAt, s.Price))
                .ToList()
        );
    }
}
