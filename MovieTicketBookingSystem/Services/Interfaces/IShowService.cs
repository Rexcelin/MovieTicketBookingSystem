
using MovieTicketBookingSystem.DTOs.Shows;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IShowService
{
    Task<ShowResponse> CreateShowAsync(CreateShowRequest request);

    Task<ShowResponse?> GetShowByIdAsync(int showId);

    Task<List<ShowResponse>> GetAllShowsAsync();

    Task<List<ShowResponse>> GetShowsByMovieIdAsync(int movieId);

    Task<List<ShowResponse>> GetShowsByScreenIdAsync(int screenId);

    Task<ShowResponse?> UpdateShowAsync(
        int showId,
        UpdateShowRequest request);

    Task<bool> DeleteShowAsync(int showId);
}
