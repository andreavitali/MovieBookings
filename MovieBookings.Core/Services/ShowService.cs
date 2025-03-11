using Microsoft.EntityFrameworkCore;
using MovieBookings.Core.Interfaces;
using MovieBookings.Data;

namespace MovieBookings.Core.Services;

public class ShowService(ApplicationDbContext DbContext) : IShowService
{
    public async Task<List<ShowResponse>> GetAllAsync()
    {
        return await GetShowCommonQuery()
            .Select(s => s.MapToShowDTO())
            .ToListAsync();
    }

    public async Task<ShowResponse?> GetByIdAsync(int Id)
    {
        var foundShow = await GetShowCommonQuery()
            .SingleOrDefaultAsync(s => s.Id == Id);

        return foundShow?.MapToShowDTO();
    }

    private IQueryable<Show> GetShowCommonQuery()
    {
        return DbContext.Shows
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.Seats)
                .ThenInclude(ss => ss.Seat);
    }
}
