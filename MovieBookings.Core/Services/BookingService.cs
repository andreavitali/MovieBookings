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
            .Select(b => b.MapToBookingDTO())
            .ToListAsync();
    }

    public async Task<BookingResponse> CreateBookingAsync(int UserId, List<BookingRequest> bookings)
    {
        // Get requested seat
        IQueryable<ShowSeat> selectedSeats = DbContext.ShowSeats
            .Where(ss => bookings.Select(br => br.SeatId).Contains(ss.Id));

        if (await selectedSeats.AnyAsync(ss => ss.Status == ShowSeatStatus.Booked))
        {
            throw new SeatAlreadyBookedException();
        }

        // Create booking
        await selectedSeats.ForEachAsync(ss => ss.Status = ShowSeatStatus.Booked);
        var booking = DbContext.Bookings.Add(new Booking
        {
            UserId = UserId,
            BookedSeats = selectedSeats.ToList()
        });

        DbContext.SaveChanges();

        // Return mapped booking
        var newBooking = await DbContext.Bookings
            .Include(b => b.BookedSeats)
            .SingleAsync(b => b.Id == booking.Entity.Id);

        return newBooking.MapToBookingDTO();

        //using var transaction = await DbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);

        //try
        //{
        //    await transaction.CommitAsync();
        //    return null;
        //}
        //catch (Exception)
        //{
        //    await transaction.RollbackAsync();
        //    throw;
        //}        
    }

    public async Task DeleteAsync(int Id)
    {
        // Delete booking
        var booking = await DbContext.Bookings.FindAsync(Id);
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
}
