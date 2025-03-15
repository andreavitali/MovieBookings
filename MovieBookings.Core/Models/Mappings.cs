using MovieBookings.Data;

namespace MovieBookings.Core.Models;

public static class Mappings
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
