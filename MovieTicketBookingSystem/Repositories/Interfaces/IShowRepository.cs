
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IShowRepository
{
    Task<Show> AddAsync(Show show);

    Task<Show?> GetByIdAsync(int showId);

    Task<List<Show>> GetAllAsync();

    Task<List<Show>> GetByMovieIdAsync(int movieId);

    Task<List<Show>> GetByScreenIdAsync(int screenId);

    Task<bool> HasScheduleOverlapAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null);

    Task UpdateAsync(Show show);

    Task<bool> DeleteAsync(int showId);
}
