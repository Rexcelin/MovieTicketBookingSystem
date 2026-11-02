using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;

namespace MovieTicketBookingSystem.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add a new movie
    public async Task<Movie> AddAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);

        await _context.SaveChangesAsync();

        return movie;
    }

    // Get movie by ID
    public async Task<Movie?> GetByIdAsync(int movieId)
    {
        return await _context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MovieId == movieId);
    }

    // Get paginated movies with optional title search
    public async Task<(IEnumerable<Movie> Movies, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        IQueryable<Movie> query = _context.Movies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(m =>
                EF.Functions.Like(m.Title, $"%{title.Trim()}%"));
        }

        int totalCount = await query.CountAsync();

        List<Movie> movies = await query
            .OrderBy(m => m.MovieId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (movies, totalCount);
    }

    // Search movies by title
    public async Task<IEnumerable<Movie>> SearchByTitleAsync(string title)
    {
        return await _context.Movies
            .AsNoTracking()
            .Where(m => EF.Functions.Like(m.Title, $"%{title.Trim()}%"))
            .OrderBy(m => m.Title)
            .ToListAsync();
    }

    // Update an existing movie
    public async Task UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);

        await _context.SaveChangesAsync();
    }

    // Delete a movie
    public async Task DeleteAsync(Movie movie)
    {
        _context.Movies.Remove(movie);

        await _context.SaveChangesAsync();
    }
}