using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Exceptions;
using MovieBookings.Core.Models;
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
    public async void GetAllByUserIdAsync_ShouldReturnAllBookingsForAUser()
    {
        var context = Fixture.CreateContext();
        var user = await context.Users.FirstAsync();
        var show = await context.Shows.Include(s => s.Movie).FirstAsync();

        var showSeats = context.ShowSeats
            .Where(ss => ss.ShowId == show.Id)
            .Take(2)
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
                Assert.Equal(show.StartAt, bs.StartAt);
                Assert.Equivalent(show.Movie, bs.Movie);
            });
        });
    }

    [Fact]
    public async void GetAllByUserIdAsync_IfUserDontExists_ShouldReturnEmptyList()
    {
        var service = new BookingService(Fixture.CreateContext());
        var bookings = await service.GetAllByUserIdAsync(int.MaxValue);
        Assert.Empty(bookings);
    }

    [Fact]
    public async void DeleteAsync_IfBookingExists_ShouldRemoveBooking_And_UpdateShowSeat()
    {
        var context = Fixture.CreateContext();
        var user = await context.Users.FirstAsync();
        var show = await context.Shows.FirstAsync();
        var showSeat = await context.ShowSeats.Where(ss => ss.ShowId == show.Id).Skip(2).Take(1).SingleAsync();
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

        var updatedShowSeat = await context.ShowSeats.SingleAsync(ss => ss.Id == showSeat.Id);
        Assert.Equal(ShowSeatStatus.Available, updatedShowSeat.Status);
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreAvailable_ShouldCreateBooking()
    {
        var context = Fixture.CreateContext();
        var user = await context.Users.FirstAsync();
        var shows = await context.Shows.Take(2).ToListAsync();
        var showSeat1 = await context.ShowSeats.FirstAsync(ss => ss.ShowId == shows[0].Id);
        var showSeat2 = await context.ShowSeats.FirstAsync(ss => ss.ShowId == shows[1].Id);

        var service = new BookingService(context);
        var bookingRequest = new List<BookingRequest> { 
            new BookingRequest(null, showSeat1.Id),
            new BookingRequest(null, showSeat2.Id)
        };
        var booking = await service.CreateBookingAsync(user.Id, bookingRequest);
        Assert.NotNull(booking);
        Assert.Equal(user.Id, booking.UserId);
        Assert.Equal(2, booking.BookedSeats.Count);
        Assert.Single(booking.BookedSeats, bs => bs.Id == showSeat1.Id);
        Assert.Single(booking.BookedSeats, bs => bs.Id == showSeat2.Id);
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatAlreadyBooked_ShouldThrowsException()
    {
        var context = Fixture.CreateContext();
        var user1 = await context.Users.FindAsync(1);
        var user2 = await context.Users.FindAsync(2);
        var show = await context.Shows.FirstAsync();
        var bookedShowSeat = await context.ShowSeats.FirstAsync();

        var service = new BookingService(context);
        var bookingRequest1 = new List<BookingRequest> { new BookingRequest(null, bookedShowSeat.Id) };
        var booking1 = await service.CreateBookingAsync(user1.Id, bookingRequest1);

        var bookingRequest2 = new List<BookingRequest> { new BookingRequest(null, bookedShowSeat.Id) };
        await Assert.ThrowsAsync<SeatAlreadyBookedException>(() => service.CreateBookingAsync(user2.Id, bookingRequest2));
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreNotSpecified_ButAvailable_ShouldCreateBooking()
    {
        var context = Fixture.CreateContext();
        var service = new BookingService(context);

        var user = await context.Users.FirstAsync();
        var shows = await context.Shows.Take(2).ToListAsync();
        var show1Seat1 = await context.ShowSeats.FirstAsync(ss => ss.ShowId == shows[0].Id);

        var oldBookingRequest = new List<BookingRequest> { new BookingRequest(shows[0].Id, show1Seat1.Id) };
        _ = await service.CreateBookingAsync(2, oldBookingRequest);

        var newBookingRequest = new List<BookingRequest> {
            new BookingRequest(shows[0].Id, null),
            new BookingRequest(shows[1].Id, null)
        };

        var booking = await service.CreateBookingAsync(user.Id, newBookingRequest);

        Assert.NotNull(booking);
        Assert.Equal(2, booking.BookedSeats.Count);
        Assert.All(booking.BookedSeats, bs => Assert.NotEqual(bs.Id, show1Seat1.Id));
    }

    [Fact]
    public async void CreateBookingAsync_WithOnlySomeSeatsSpecified_ButAvailable_ShouldCreateBooking()
    {
        var context = Fixture.CreateContext();
        var service = new BookingService(context);

        var user = await context.Users.FirstAsync();
        var show = await context.Shows.FirstAsync();
        var show1Seat1 = await context.ShowSeats.FirstAsync(ss => ss.ShowId == show.Id);

        var newBookingRequest = new List<BookingRequest> {
            new BookingRequest(null, show1Seat1.Id),
            new BookingRequest(show.Id, null)
        };

        var booking = await service.CreateBookingAsync(user.Id, newBookingRequest);

        Assert.NotNull(booking);
        Assert.Equal(2, booking.BookedSeats.Count);
        Assert.Single(booking.BookedSeats, bs => bs.Id == show1Seat1.Id);
    }

    [Fact]
    public async void CreateBookingAsync_IfSeatsAreNotSpecified_AndNotAllAvailabile_ShouldThrowsException()
    {
        var context = Fixture.CreateContext();
        var service = new BookingService(context);

        var user = await context.Users.FirstAsync();
        var show = await context.Shows.Include(s => s.Seats).FirstAsync();

        var oldBookingRequest = Enumerable.Range(1, show.Seats.Count - 1)
            .Select(_ => new BookingRequest(show.Id, null)).ToList();
        _ = await service.CreateBookingAsync(2, oldBookingRequest);

        var newBookingRequest = new List<BookingRequest> {
            new BookingRequest(show.Id, null),
            new BookingRequest(show.Id, null)
        };

        await Assert.ThrowsAsync<SeatAlreadyBookedException>(() => service.CreateBookingAsync(user.Id, newBookingRequest));
    }
}