using Microsoft.EntityFrameworkCore;
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
                .ThenInclude(bs => bs.Seat)
            .Select(b => b.MapToBookingDTO())
            .ToListAsync();
    }

    public Task<BookingResponse> CreateBookingAsync(int UserId, List<BookingRequest> bookings)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int Id)
    {
        // Delete booking
        var booking = await DbContext.Bookings.FindAsync(Id);
        if (booking is not null)
        {
            //using var transaction = await DbContext.Database.BeginTransactionAsync();
            var bookedSeatsKeys = booking.BookedSeats.Select(bs => bs.Id).ToList();

            DbContext.Bookings.Remove(booking);
            //await DbContext.SaveChangesAsync();

            // Restore show seats status
            var showSeatsToUpdate = await DbContext.ShowSeats
                .Where(ss => bookedSeatsKeys.Contains(ss.Id))
                .ToListAsync();

            showSeatsToUpdate.ForEach(ss => ss.Status = ShowSeatStatus.Available);
            await DbContext.SaveChangesAsync();

            //await transaction.CommitAsync();
        }
    }
}
