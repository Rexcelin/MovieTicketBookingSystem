using MovieTicketBookingSystem.DTOs.Screens;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;
    private readonly ITheatreRepository _theatreRepository;

    public ScreenService(
        IScreenRepository screenRepository,
        ITheatreRepository theatreRepository)
    {
        _screenRepository = screenRepository;
        _theatreRepository = theatreRepository;
    }

    public async Task<ScreenResponse> CreateScreenAsync(
        CreateScreenRequest request)
    {
        if (request.TheatreId <= 0)
        {
            throw new ArgumentException("Invalid theatre ID.");
        }

        var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);

        if (theatre == null)
        {
            throw new ArgumentException("The specified theatre does not exist.");
        }

        ValidateScreenName(request.ScreenName);

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName.Trim()
        };

        var createdScreen = await _screenRepository.AddAsync(screen);

        return MapToResponse(createdScreen);
    }

    public async Task<List<ScreenResponse>> GetAllScreensAsync()
    {
        var screens = await _screenRepository.GetAllAsync();

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> GetScreenByIdAsync(int screenId)
    {
        if (screenId <= 0)
        {
            throw new ArgumentException("Invalid screen ID.");
        }

        var screen = await _screenRepository.GetByIdAsync(screenId);

        return screen == null ? null : MapToResponse(screen);
    }

    public async Task<List<ScreenResponse>> GetScreensByTheatreIdAsync(
        int theatreId)
    {
        if (theatreId <= 0)
        {
            throw new ArgumentException("Invalid theatre ID.");
        }

        var theatre = await _theatreRepository.GetByIdAsync(theatreId);

        if (theatre == null)
        {
            throw new ArgumentException("The specified theatre does not exist.");
        }

        var screens = await _screenRepository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> UpdateScreenAsync(
        int screenId,
        UpdateScreenRequest request)
    {
        if (screenId <= 0)
        {
            throw new ArgumentException("Invalid screen ID.");
        }

        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
        {
            return null;
        }

        if (request.TheatreId <= 0)
        {
            throw new ArgumentException("Invalid theatre ID.");
        }

        var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);

        if (theatre == null)
        {
            throw new ArgumentException("The specified theatre does not exist.");
        }

        ValidateScreenName(request.ScreenName);

        screen.TheatreId = request.TheatreId;
        screen.ScreenName = request.ScreenName.Trim();

        await _screenRepository.UpdateAsync(screen);

        return MapToResponse(screen);
    }

    public async Task<bool> DeleteScreenAsync(int screenId)
    {
        if (screenId <= 0)
        {
            throw new ArgumentException("Invalid screen ID.");
        }

        return await _screenRepository.DeleteAsync(screenId);
    }

    private static void ValidateScreenName(string? screenName)
    {
        if (string.IsNullOrWhiteSpace(screenName))
        {
            throw new ArgumentException("Screen name is required.");
        }

        if (screenName.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Screen name cannot exceed 100 characters.");
        }
    }

    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            ScreenName = screen.ScreenName
        };
    }
}