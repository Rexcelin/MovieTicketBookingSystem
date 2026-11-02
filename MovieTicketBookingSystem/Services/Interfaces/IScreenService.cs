using MovieTicketBookingSystem.DTOs.Screens;

namespace MovieTicketBookingSystem.Services.Interfaces;

public interface IScreenService
{
    Task<ScreenResponse> CreateScreenAsync(CreateScreenRequest request);

    Task<List<ScreenResponse>> GetAllScreensAsync();

    Task<ScreenResponse?> GetScreenByIdAsync(int screenId);

    Task<List<ScreenResponse>> GetScreensByTheatreIdAsync(int theatreId);

    Task<ScreenResponse?> UpdateScreenAsync(
        int screenId,
        UpdateScreenRequest request);

    Task<bool> DeleteScreenAsync(int screenId);
}