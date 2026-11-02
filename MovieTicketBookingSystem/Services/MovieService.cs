using MovieTicketBookingSystem.DTOs.Movies;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    // Create a new movie
    public async Task<MovieResponse> CreateMovieAsync(
        CreateMovieRequest request)
    {
        ValidateMovieDetails(
            request.Title,
            request.DurationMinutes,
            request.ReleaseDate,
            request.Language,
            request.Genre);

        var movie = new Movie
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language.Trim(),
            Genre = request.Genre.Trim()
        };

        var createdMovie = await _movieRepository.AddAsync(movie);

        return MapToResponse(createdMovie);
    }

    // Get movie by ID
    public async Task<MovieResponse?> GetMovieByIdAsync(int movieId)
    {
        if (movieId <= 0)
        {
            throw new ArgumentException("Movie ID must be greater than zero.");
        }

        var movie = await _movieRepository.GetByIdAsync(movieId);

        return movie == null ? null : MapToResponse(movie);
    }

    // Get paginated movies
    public async Task<(IEnumerable<MovieResponse> Movies, int TotalCount)>
        GetMoviesAsync(int pageNumber, int pageSize, string? title)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be at least 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException(
                "Page size must be between 1 and 100.");
        }

        var result = await _movieRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            title);

        var movies = result.Movies.Select(MapToResponse);

        return (movies, result.TotalCount);
    }

    // Search movies by title
    public async Task<IEnumerable<MovieResponse>> SearchMoviesAsync(
        string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Search title cannot be empty.");
        }

        var movies = await _movieRepository.SearchByTitleAsync(title.Trim());

        return movies.Select(MapToResponse);
    }

    // Update movie details
    public async Task<MovieResponse?> UpdateMovieAsync(
        int movieId,
        UpdateMovieRequest request)
    {
        if (movieId <= 0)
        {
            throw new ArgumentException("Movie ID must be greater than zero.");
        }

        ValidateMovieDetails(
            request.Title,
            request.DurationMinutes,
            request.ReleaseDate,
            request.Language,
            request.Genre);

        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
        {
            return null;
        }

        movie.Title = request.Title.Trim();
        movie.Description = request.Description?.Trim();
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language.Trim();
        movie.Genre = request.Genre.Trim();

        await _movieRepository.UpdateAsync(movie);

        return MapToResponse(movie);
    }

    // Delete movie
    public async Task<bool> DeleteMovieAsync(int movieId)
    {
        if (movieId <= 0)
        {
            throw new ArgumentException("Movie ID must be greater than zero.");
        }

        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
        {
            return false;
        }

        await _movieRepository.DeleteAsync(movie);

        return true;
    }

    // Validate movie details
    private static void ValidateMovieDetails(
        string title,
        int durationMinutes,
        DateTime releaseDate,
        string language,
        string genre)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Movie title is required.");
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentException(
                "Movie duration must be greater than zero.");
        }

        if (releaseDate == default)
        {
            throw new ArgumentException("Release date is required.");
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            throw new ArgumentException("Movie language is required.");
        }

        if (string.IsNullOrWhiteSpace(genre))
        {
            throw new ArgumentException("Movie genre is required.");
        }
    }

    // Convert Movie entity to MovieResponse DTO
    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}