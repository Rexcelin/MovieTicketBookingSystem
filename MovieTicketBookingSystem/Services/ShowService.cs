
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Data;
using MovieTicketBookingSystem.DTOs.Shows;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class ShowService : IShowService
{
    private readonly ApplicationDbContext _context;
    private readonly IShowRepository _showRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IScreenRepository _screenRepository;
    private readonly ISeatRepository _seatRepository;

    public ShowService(
        ApplicationDbContext context,
        IShowRepository showRepository,
        IMovieRepository movieRepository,
        IScreenRepository screenRepository,
        ISeatRepository seatRepository)
    {
        _context = context;
        _showRepository = showRepository;
        _movieRepository = movieRepository;
        _screenRepository = screenRepository;
        _seatRepository = seatRepository;
    }

    public async Task<ShowResponse> CreateShowAsync(
        CreateShowRequest request)
    {
        await ValidateReferencesAsync(
            request.MovieId,
            request.ScreenId);

        ValidateShowDetails(
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        bool overlap = await _showRepository.HasScheduleOverlapAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime);

        if (overlap)
        {
            throw new ArgumentException(
                "Another show is already scheduled during this time on this screen.");
        }

        var seats = await _seatRepository.GetByScreenIdAsync(
            request.ScreenId);

        if (seats.Count == 0)
        {
            throw new ArgumentException(
                "No seats are configured for this screen. Add seats before creating a show.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            await _context.Shows.AddAsync(show);
            await _context.SaveChangesAsync();

            var showSeats = seats.Select(seat => new ShowSeat
            {
                ShowId = show.ShowId,
                SeatId = seat.SeatId,
                Status = "Available"
            }).ToList();

            await _context.ShowSeats.AddRangeAsync(showSeats);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var createdShow = await _showRepository.GetByIdAsync(
                show.ShowId);

            return MapToResponse(createdShow!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ShowResponse?> GetShowByIdAsync(int showId)
    {
        if (showId <= 0)
            throw new ArgumentException("Invalid show ID.");

        var show = await _showRepository.GetByIdAsync(showId);

        return show == null ? null : MapToResponse(show);
    }

    public async Task<List<ShowResponse>> GetAllShowsAsync()
    {
        var shows = await _showRepository.GetAllAsync();

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> GetShowsByMovieIdAsync(
        int movieId)
    {
        if (movieId <= 0)
            throw new ArgumentException("Invalid movie ID.");

        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
            throw new ArgumentException(
                $"Movie with ID {movieId} does not exist.");

        var shows = await _showRepository.GetByMovieIdAsync(movieId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> GetShowsByScreenIdAsync(
        int screenId)
    {
        if (screenId <= 0)
            throw new ArgumentException("Invalid screen ID.");

        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new ArgumentException(
                $"Screen with ID {screenId} does not exist.");

        var shows = await _showRepository.GetByScreenIdAsync(screenId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> UpdateShowAsync(
        int showId,
        UpdateShowRequest request)
    {
        if (showId <= 0)
            throw new ArgumentException("Invalid show ID.");

        var show = await _context.Shows
            .FirstOrDefaultAsync(s => s.ShowId == showId);

        if (show == null)
            return null;

        await ValidateReferencesAsync(
            request.MovieId,
            request.ScreenId);

        ValidateShowDetails(
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        bool overlap = await _showRepository.HasScheduleOverlapAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            showId);

        if (overlap)
        {
            throw new ArgumentException(
                "Another show is already scheduled during this time on this screen.");
        }

        bool hasBookings = await _context.Bookings
            .AnyAsync(b => b.ShowId == showId);

        if (hasBookings)
        {
            throw new ArgumentException(
                "A show with existing bookings cannot be updated.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            if (show.ScreenId != request.ScreenId)
            {
                var oldShowSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == showId)
                    .ToListAsync();

                _context.ShowSeats.RemoveRange(oldShowSeats);

                var newSeats = await _seatRepository.GetByScreenIdAsync(
                    request.ScreenId);

                if (newSeats.Count == 0)
                {
                    throw new ArgumentException(
                        "No seats are configured for the selected screen.");
                }

                var newShowSeats = newSeats.Select(seat => new ShowSeat
                {
                    ShowId = showId,
                    SeatId = seat.SeatId,
                    Status = "Available"
                }).ToList();

                await _context.ShowSeats.AddRangeAsync(newShowSeats);
            }

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var updatedShow = await _showRepository.GetByIdAsync(showId);

            return updatedShow == null
                ? null
                : MapToResponse(updatedShow);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteShowAsync(int showId)
    {
        if (showId <= 0)
            throw new ArgumentException("Invalid show ID.");

        var show = await _showRepository.GetByIdAsync(showId);

        if (show == null)
            return false;

        bool hasBookings = await _context.Bookings
            .AnyAsync(b => b.ShowId == showId);

        if (hasBookings)
        {
            throw new ArgumentException(
                "A show with existing bookings cannot be deleted.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var showSeats = await _context.ShowSeats
                .Where(ss => ss.ShowId == showId)
                .ToListAsync();

            _context.ShowSeats.RemoveRange(showSeats);
            await _context.SaveChangesAsync();

            bool deleted = await _showRepository.DeleteAsync(showId);

            await transaction.CommitAsync();

            return deleted;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task ValidateReferencesAsync(
        int movieId,
        int screenId)
    {
        if (movieId <= 0)
            throw new ArgumentException("Invalid movie ID.");

        if (screenId <= 0)
            throw new ArgumentException("Invalid screen ID.");

        var movie = await _movieRepository.GetByIdAsync(movieId);

        if (movie == null)
            throw new ArgumentException(
                $"Movie with ID {movieId} does not exist.");

        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
            throw new ArgumentException(
                $"Screen with ID {screenId} does not exist.");
    }

    private static void ValidateShowDetails(
        DateTime startTime,
        DateTime endTime,
        decimal ticketPrice)
    {
        if (startTime == default || endTime == default)
            throw new ArgumentException(
                "Start time and end time are required.");

        if (endTime <= startTime)
            throw new ArgumentException(
                "End time must be later than start time.");

        if (ticketPrice < 0 || ticketPrice > 99999999.99m)
            throw new ArgumentException(
                "Ticket price must be between 0 and 99999999.99.");
    }

    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            MovieTitle = show.Movie.Title,
            ScreenId = show.ScreenId,
            ScreenName = show.Screen.ScreenName,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice
        };
    }
}
