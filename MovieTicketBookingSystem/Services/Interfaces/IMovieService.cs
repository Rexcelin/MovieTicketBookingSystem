using MovieTicketBookingSystem.DTOs.Movies;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IMovieService
{
    // Create a new movie
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);

    // Get movie by ID
    Task<MovieResponse?> GetMovieByIdAsync(int movieId);

    // Get paginated movies
    Task<(IEnumerable<MovieResponse> Movies, int TotalCount)> GetMoviesAsync(
        int pageNumber,
        int pageSize,
        string? title);

    // Search movies by title
    Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string title);

    // Update movie details
    Task<MovieResponse?> UpdateMovieAsync(
        int movieId,
        UpdateMovieRequest request);

    // Delete movie
    Task<bool> DeleteMovieAsync(int movieId);
}