using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Interfaces;
using MovieBookings.Data;

namespace MovieBookings.Core.Services;

public class BookingService(ApplicationDbContext DbContext) : IBookingService
{
    public async Task<List<BookingResponse>> GetAllByUserIdAsync(int UserId)
    {
        return await DbContext.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == UserId)
            .Include(b => b.BookedSeats)
                .ThenInclude(bs => bs.Show)
                    .ThenInclude(s => s.Movie)
            .Select(b => b.MapToBookingDTO())
            .ToListAsync();
    }

    public async Task<BookingResponse> CreateBookingAsync(int UserId, List<BookingRequest> bookings)
    {
        // Get specified seats
        var selectedSeatIds = bookings.Where(br => br.SeatId != null).Select(br => br.SeatId).ToList();

        IQueryable<ShowSeat> selectedSeatsQuery = DbContext.ShowSeats
            .Where(ss => selectedSeatIds.Contains(ss.Id));

        // Get unspecified seats
        Dictionary<int, int> unspecifedSeatsPerShow = bookings
            .Where(br => br.SeatId == null)
            .GroupBy(br => br.ShowId.Value)
            .ToDictionary(br => br.Key, br => br.Count());

        foreach (var seatGroup in unspecifedSeatsPerShow)
        {
            // Get first N available seats for each show
            var unspecifiedSeats = DbContext.ShowSeats
                .Where(ss => !selectedSeatIds.Contains(ss.Id))
                .Where(ss => ss.ShowId == seatGroup.Key)
                .Where(ss => ss.Status == ShowSeatStatus.Available)
                .Take(seatGroup.Value);

            selectedSeatsQuery = selectedSeatsQuery.Union(unspecifiedSeats);
        }

        // Check availability
        var selectedSeats = await selectedSeatsQuery.AsNoTracking().ToListAsync();

        if (selectedSeats.Count != bookings.Count || selectedSeats.Any(ss => ss.Status == ShowSeatStatus.Booked))
        {
            throw new SeatAlreadyBookedException();
        }

        // Create booking
        await selectedSeatsQuery.ForEachAsync(ss => ss.Status = ShowSeatStatus.Booked);
        var booking = DbContext.Bookings.Add(new Booking
        {
            UserId = UserId,
            TotalPrice = selectedSeats.Sum(ss => ss.Price),
            BookedSeats = selectedSeatsQuery.ToList()
        });

        DbContext.SaveChanges();

        // Return mapped booking
        var newBooking = await DbContext.Bookings
            .Include(b => b.BookedSeats)
                .ThenInclude(bs => bs.Show)
                    .ThenInclude(s => s.Movie)
            .SingleAsync(b => b.Id == booking.Entity.Id);

        return newBooking.MapToBookingDTO();    
    }

    public async Task DeleteAsync(int Id)
    {
        // Delete booking
        var booking = await DbContext.Bookings
            .Include(b => b.BookedSeats)
            .SingleOrDefaultAsync(b => b.Id == Id);

        if (booking is not null)
        {
            var bookedSeatsKeys = booking.BookedSeats.Select(bs => bs.Id).ToList();

            DbContext.Bookings.Remove(booking);

            // Restore show seats status
            var showSeatsToUpdate = await DbContext.ShowSeats
                .Where(ss => bookedSeatsKeys.Contains(ss.Id))
                .ToListAsync();

            showSeatsToUpdate.ForEach(ss => ss.Status = ShowSeatStatus.Available);
            await DbContext.SaveChangesAsync();
        }
    }

    public async Task<BookingResponse> GetByIdAsync(int Id)
    {
        var booking = await DbContext.Bookings
            .AsNoTracking()
            .Include(b => b.BookedSeats)
                .ThenInclude(bs => bs.Show)
                    .ThenInclude(s => s.Movie)
            .SingleOrDefaultAsync(b => b.Id == Id);

        return booking?.MapToBookingDTO();
    }
}
