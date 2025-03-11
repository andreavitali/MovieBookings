using Microsoft.EntityFrameworkCore;
using MovieBookings.Core;
using MovieBookings.Core.Services;
using MovieBookings.Data;

namespace MovieBookings.UnitTests;

public class BookingServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    public BookingServiceTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    public TestDatabaseFixture Fixture { get; }

    public void Dispose()
    {
        var context = Fixture.CreateContext();
        context.Bookings.RemoveRange(context.Bookings);
        context.ShowSeats.ExecuteUpdate(setter => setter.SetProperty(ss => ss.Status, ShowSeatStatus.Available));
        context.SaveChanges();
    }

    [Fact]
    public async void GetAllByUserIdAsync_ReturnsAllBookingsForAUser()
    {
        var context = Fixture.CreateContext();
        var user = await context.Users.FirstAsync();
        var show = await context.Shows.FirstAsync();
        var seats = await context.Seats.Take(2).ToListAsync();

        var showSeats = context.ShowSeats
            .Where(ss => ss.ShowId == show.Id && seats.Select(s => s.Id).Contains(ss.SeatId))
            .AsEnumerable()
            .Select(ss => { ss.Status = ShowSeatStatus.Booked; return ss; });

        var booking = await context.Bookings.AddAsync(new Booking
        {
            UserId = user.Id,
            BookedSeats = showSeats.ToList()
        });

        await context.SaveChangesAsync();

        showSeats = context.ShowSeats.ToList();

        var service = new BookingService(context);
        var bookings = await service.GetAllByUserIdAsync(user.Id);

        Assert.IsType<List<BookingResponse>>(bookings);
        Assert.Single(bookings);
        Assert.All(bookings, b =>
        {
            Assert.Equal(user.Id, b.UserId);
            Assert.Equal(2, b.BookedSeats.Count);
            Assert.All(b.BookedSeats, bs =>
            {
                Assert.Equal(ShowSeatStatus.Booked, bs.Status);
            });
        });
    }

    [Fact]
    public async void GetAllByUserIdAsync_IfUserDontExists_ReturnsEmptyList()
    {
        var service = new BookingService(Fixture.CreateContext());
        var bookings = await service.GetAllByUserIdAsync(int.MaxValue);
        Assert.Empty(bookings);
    }

    [Fact]
    public async void DeleteAsync_IfBookingExists_RemoveBooking_AndUpdateShowSeat()
    {
        var context = Fixture.CreateContext();
        var user = await context.Users.FirstAsync();
        var show = await context.Shows.FirstAsync();
        var seat = await context.Seats.Skip(2).Take(1).SingleAsync();
        var showSeat = await context.ShowSeats.SingleAsync(ss => ss.ShowId == show.Id && ss.SeatId == seat.Id);
        showSeat.Status = ShowSeatStatus.Booked;
        var booking = context.Bookings.Add(new Booking
        {
            UserId = user.Id,
            BookedSeats = [showSeat]
        });
        await context.SaveChangesAsync();

        var service = new BookingService(context);
        var bookings = await service.GetAllByUserIdAsync(user.Id);
        var bookingIdToDelete = bookings[0].Id;
        await service.DeleteAsync(bookingIdToDelete);

        bookings = await service.GetAllByUserIdAsync(user.Id);
        Assert.Empty(bookings);

        var updatedShowSeat = await context.ShowSeats.SingleAsync(ss => ss.ShowId == show.Id && ss.SeatId == seat.Id);
        Assert.Equal(ShowSeatStatus.Available, updatedShowSeat?.Status);
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreAvailable_CreateBooking()
    {
        //var context = Fixture.CreateContext();
        //var user = await context.Users.FirstAsync();
        //var show = await context.Shows.FirstAsync();
        //var seat = await context.Seats.Skip(3).SingleAsync();

        //var service = new BookingService(context);
        //var bookingRequest = new List<BookingRequest> { new BookingRequest(show.Id, seat.Id) };
        //var booking = await service.CreateBookingAsync(user.Id, bookingRequest);
        //Assert.NotNull(booking);
        //Assert.Equal(user.Id, booking.UserId);
        //Assert.Single(booking.BookedSeats);

        //await service.DeleteAsync(booking.Id);
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreNotSpecified_ButAvailable_CreateBooking()
    {

    }

    [Fact]
    public async void CreateBookingAsync_IfSomeSeatsAreSpecified_AndOtherNot_ButAvailable_CreateBooking()
    {

    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreNotSpecified_AndNoAvailabilty_ThrowsException()
    {

    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreAlreadyBooked_ThrowsException()
    {

    }
}