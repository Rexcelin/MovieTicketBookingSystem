using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Repositories.Interfaces;

public interface IMovieRepository
{
    // Add a new movie
    Task<Movie> AddAsync(Movie movie);

    // Get a movie by ID
    Task<Movie?> GetByIdAsync(int movieId);

    // Get paginated movies with optional title filtering
    Task<(IEnumerable<Movie> Movies, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? title);

    // Search movies by title
    Task<IEnumerable<Movie>> SearchByTitleAsync(string title);

    // Update an existing movie
    Task UpdateAsync(Movie movie);

    // Delete a movie
    Task DeleteAsync(Movie movie);
}