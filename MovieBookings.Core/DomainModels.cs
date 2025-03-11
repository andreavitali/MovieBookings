using MovieBookings.Data;
using System.Linq;

namespace MovieBookings.Core;

public record Seat(int Id, char Row, int Number, ShowSeatStatus Status);

public record ShowResponse(int Id, Movie Movie, DateTime StartAt, List<Seat> Seats);
public record BookingResponse(int Id, int UserId, double TotalPrice, List<Seat> BookedSeats);
public record BookingRequest(int ShowId, int? SeatId);

public static class DTOMapping
{
    public static ShowResponse MapToShowDTO(this Show showEntity)
    {
        return new ShowResponse(
            showEntity.Id,
            showEntity.Movie,
            showEntity.StartAt,
            showEntity.Seats.Select(s => new Seat(s.SeatId, s.Seat.Row, s.Seat.Number, s.Status)).ToList()
        );
    }

    public static BookingResponse MapToBookingDTO(this Booking bookingEntity)
    {
        return new BookingResponse(
            bookingEntity.Id,
            bookingEntity.UserId,
            bookingEntity.TotalPrice,
            bookingEntity.BookedSeats.Select(s => new Seat(s.SeatId, s.Seat.Row, s.Seat.Number, s.Status)).ToList()
        );
    }
}
