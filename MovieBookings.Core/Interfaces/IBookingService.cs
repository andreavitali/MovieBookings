namespace MovieBookings.Core.Interfaces;

public interface IBookingService
{
    Task<List<BookingResponse>> GetAllByUserIdAsync(int UserId);
    Task<BookingResponse> CreateBookingAsync(int UserId, List<BookingRequest> bookings);
    Task DeleteAsync(int Id);
}
