using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Interfaces;
using MovieBookings.Data;

namespace MovieBookings.Core.Services;

public class ShowService(ApplicationDbContext DbContext) : IShowService
{
    public async Task<List<ShowDTO>> GetAllShowsAsync()
    {
        return await DbContext.Shows
            .Include(s => s.Movie)
            .Include(s => s.BookedSeats)
                .ThenInclude(bs => bs.Seat)
            .Select(s => s.MapToShowDTO())
            .ToListAsync();
    }

    public async Task<ShowDTO?> GetShowByIdAsync(int Id)
    {
        var foundShow = await DbContext.Shows
            .Include(s => s.Movie)
            .Include(s => s.BookedSeats)
                .ThenInclude(bs => bs.Seat)
            .SingleOrDefaultAsync(s => s.Id == Id);

        return foundShow?.MapToShowDTO();
    }

    private IQueryable<Show> GetShowCommonQuery()
    {
        return DbContext.Shows
            .Include(s => s.Movie)
            .Include(s => s.BookedSeats)
                .ThenInclude(bs => bs.Seat);
    }
}
