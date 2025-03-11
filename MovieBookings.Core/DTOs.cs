using MovieBookings.Data;

namespace MovieBookings.Core;

public record ShowDTO(int Id, MovieDTO Movie, DateTime StartAt, int TotalSeats, int AvailableSeats, List<BookedSeatDTO> BookedSeats);
public record MovieDTO(int Id, string Title, int Duration);
public record BookedSeatDTO(int Id, int BookingId, string Seat);

public static class DTOMapping
{
    public static ShowDTO MapToShowDTO(this Show showEntity)
    {
        return new ShowDTO(
            showEntity.Id,
            new MovieDTO(showEntity.Movie.Id, showEntity.Movie.Title, showEntity.Movie.Duration),
            showEntity.StartAt,
            showEntity.TotalSeat,
            showEntity.TotalSeat - showEntity.BookedSeats.Count,
            showEntity.BookedSeats.Select(bs => new BookedSeatDTO(bs.Id, bs.BookingId, $"{bs.Seat.Row}-{bs.Seat.Number}")).ToList()
        );
    }
}
