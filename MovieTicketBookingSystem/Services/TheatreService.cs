using MovieTicketBookingSystem.DTOs.Theatres;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class TheatreService : ITheatreService
{
    private readonly ITheatreRepository _repository;

    public TheatreService(ITheatreRepository repository)
    {
        _repository = repository;
    }

    public async Task<TheatreResponse> CreateTheatreAsync(
        CreateTheatreRequest request)
    {
        var theatre = new Theatre
        {
            Name = request.Name.Trim(),
            Location = request.Location.Trim()
        };

        var createdTheatre = await _repository.AddAsync(theatre);

        return MapToResponse(createdTheatre);
    }

    public async Task<List<TheatreResponse>> GetAllTheatresAsync()
    {
        var theatres = await _repository.GetAllAsync();

        return theatres.Select(MapToResponse).ToList();
    }

    public async Task<TheatreResponse?> GetTheatreByIdAsync(int theatreId)
    {
        var theatre = await _repository.GetByIdAsync(theatreId);

        return theatre == null ? null : MapToResponse(theatre);
    }

    public async Task<TheatreResponse?> UpdateTheatreAsync(
        int theatreId,
        UpdateTheatreRequest request)
    {
        var theatre = await _repository.GetByIdAsync(theatreId);

        if (theatre == null)
        {
            return null;
        }

        theatre.Name = request.Name.Trim();
        theatre.Location = request.Location.Trim();

        await _repository.UpdateAsync(theatre);

        return MapToResponse(theatre);
    }

    public async Task<bool> DeleteTheatreAsync(int theatreId)
    {
        return await _repository.DeleteAsync(theatreId);
    }

    private static TheatreResponse MapToResponse(Theatre theatre)
    {
        return new TheatreResponse
        {
            TheatreId = theatre.TheatreId,
            Name = theatre.Name,
            Location = theatre.Location
        };
    }
}