namespace MovieBookings.Core.Interfaces
{
    public interface IShowService
    {
        Task<List<ShowDTO>> GetAllShowsAsync();
        Task<ShowDTO?> GetShowByIdAsync(int Id);
    }
}
