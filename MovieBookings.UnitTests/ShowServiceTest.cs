using Microsoft.EntityFrameworkCore;
using MovieBookings.Core;
using MovieBookings.Core.Services;
using MovieBookings.Data;

namespace MovieBookings.UnitTests;

public class ShowServiceTest : IClassFixture<TestDatabaseFixture>
{
    public ShowServiceTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    public TestDatabaseFixture Fixture { get; }

    [Fact]
    public async void GetAllShowsAsync_ReturnsAllShows()
    {
        var service = new ShowService(Fixture.CreateContext());
        var shows = await service.GetAllShowsAsync();
        Assert.Equal(2, shows.Count);
        Assert.IsType<List<ShowDTO>>(shows);
    }

    [Fact]
    public async Task GetShowByIdAsync_IfExists_ReturnsTheShow()
    {
        var service = new ShowService(Fixture.CreateContext());
        var show = await service.GetShowByIdAsync(1);
        Assert.NotNull(show);
        Assert.IsType<ShowDTO>(show);
        Assert.IsType<MovieDTO>(show.Movie);
        Assert.Equal(7, show.TotalSeats);
        Assert.Equal(7, show.AvailableSeats);
        Assert.Empty(show.BookedSeats);
    }

    [Fact]
    public async void GetShowByIdAsync_IfNotExists_ReturnsNull()
    {
        var service = new ShowService(Fixture.CreateContext());
        var show = await service.GetShowByIdAsync(int.MaxValue);
        Assert.Null(show);
    }

    [Fact]
    public async Task GetShowByIdAsync_IfExists_AndHasBookedSeats_ReturnsBookedSeats()
    {
        var context = Fixture.CreateContext();

        // Add BookedSeats
        var seat = await context.Seats.FirstAsync();
        var showId = 1;
        var userId = 1;

        var booking = context.Bookings.Add(new Booking {
            UserId = userId,
            BookedSeats = [new BookedSeat { SeatId = seat.Id, ShowId = showId }]
        });

        await context.SaveChangesAsync();

        var expectedSeatDescription = new Seat() { Row = seat.Row, Number = seat.Number }.ToString();

        var service = new ShowService(context);
        var show = await service.GetShowByIdAsync(showId);

        Assert.NotNull(show);
        Assert.Equal(7, show.TotalSeats);
        Assert.Equal(6, show.AvailableSeats);
        Assert.Single(show.BookedSeats);
        Assert.IsType<BookedSeatDTO>(show.BookedSeats[0]);
        Assert.Collection(show.BookedSeats, bs =>
        {
            Assert.Equal(1, bs.Id);
            Assert.Equal(1, bs.BookingId);
            Assert.Equal(expectedSeatDescription, bs.Seat);
        });
    }
}