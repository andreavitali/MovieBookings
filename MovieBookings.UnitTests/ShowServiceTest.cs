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
    public async void GetAllShowsAsync_ShouldReturnAllShows()
    {
        var service = new ShowService(Fixture.CreateContext());

        var shows = await service.GetAllAsync();

        Assert.Equal(2, shows.Count);
        Assert.IsType<List<ShowResponse>>(shows);
    }

    [Fact]
    public async void GetShowByIdAsync_IfExists_ShouldReturnTheShow()
    {
        var context = Fixture.CreateContext();
        var service = new ShowService(context);

        var show = await service.GetByIdAsync(2);

        Assert.NotNull(show);
        Assert.IsType<ShowResponse>(show);
        Assert.IsType<Movie>(show.Movie);
        Assert.All(show.Seats, s =>
        {
            Assert.IsType<Seat>(s);
            Assert.Equal(ShowSeatStatus.Available, s.Status);
        });
    }

    [Fact]
    public async void GetShowByIdAsync_IfNotExists_ShouldReturnNull()
    {
        var service = new ShowService(Fixture.CreateContext());

        var show = await service.GetByIdAsync(int.MaxValue);

        Assert.Null(show);
    }

    [Fact]
    public async void GetShowByIdAsync_IfExists_AndHasBookedSeats_ShouldReturnSeats()
    {
        var context = Fixture.CreateContext();

        var user = await context.Users.FirstAsync();
        var firstShow = await context.Shows.FirstAsync();
        var showSeat = await context.ShowSeats.FirstAsync(ss => ss.ShowId == firstShow.Id);
        showSeat.Status = ShowSeatStatus.Booked;
        var booking = await context.Bookings.AddAsync(new Booking
        {
            UserId = user.Id,
            BookedSeats = [showSeat]
        });

        await context.SaveChangesAsync();

        var service = new ShowService(context);

        var show = await service.GetByIdAsync(firstShow.Id);

        Assert.NotNull(show);
        Assert.All(show.Seats, bs =>
        {
            Assert.Equal((bs.Id == showSeat.Id ? ShowSeatStatus.Booked : ShowSeatStatus.Available), bs.Status);
        });
    }
}