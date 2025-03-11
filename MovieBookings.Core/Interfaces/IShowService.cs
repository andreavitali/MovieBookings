namespace MovieBookings.Core.Interfaces;

public interface IShowService
{
    Task<List<ShowResponse>> GetAllAsync();
    Task<ShowResponse?> GetByIdAsync(int Id);
}
