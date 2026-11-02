using MovieTicketBookingSystem.DTOs.Seats;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    private readonly IScreenRepository _screenRepository;

    public SeatService(
        ISeatRepository seatRepository,
        IScreenRepository screenRepository)
    {
        _seatRepository = seatRepository;
        _screenRepository = screenRepository;
    }

    // Create a single seat
    public async Task<SeatResponse> CreateSeatAsync(
        CreateSeatRequest request)
    {
        await ValidateScreenAsync(request.ScreenId);

        ValidateSeatDetails(
            request.RowName,
            request.SeatNumber,
            request.SeatType);

        bool exists = await _seatRepository.ExistsAsync(
            request.ScreenId,
            request.RowName.Trim().ToUpperInvariant(),
            request.SeatNumber);

        if (exists)
        {
            throw new ArgumentException(
                "A seat with this row and seat number already exists in this screen.");
        }

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = request.RowName.Trim().ToUpperInvariant(),
            SeatNumber = request.SeatNumber,
            SeatType = request.SeatType.Trim()
        };

        var createdSeat = await _seatRepository.AddAsync(seat);

        return MapToResponse(createdSeat);
    }

    // Create multiple seats in bulk
    public async Task<List<SeatResponse>> CreateSeatsBulkAsync(
        List<CreateSeatRequest> requests)
    {
        if (requests == null || requests.Count == 0)
        {
            throw new ArgumentException(
                "At least one seat must be provided.");
        }

        int screenId = requests[0].ScreenId;

        if (requests.Any(r => r.ScreenId != screenId))
        {
            throw new ArgumentException(
                "All seats in a bulk request must belong to the same screen.");
        }

        await ValidateScreenAsync(screenId);

        var normalizedRequests = requests.Select(r => new
        {
            Request = r,
            RowName = r.RowName?.Trim().ToUpperInvariant()
        }).ToList();

        foreach (var item in normalizedRequests)
        {
            ValidateSeatDetails(
                item.RowName,
                item.Request.SeatNumber,
                item.Request.SeatType);
        }

        // Check duplicate coordinates inside the request
        var duplicateInRequest = normalizedRequests
            .GroupBy(x => new
            {
                x.RowName,
                x.Request.SeatNumber
            })
            .Any(g => g.Count() > 1);

        if (duplicateInRequest)
        {
            throw new ArgumentException(
                "The bulk request contains duplicate seat coordinates.");
        }

        // Check duplicate coordinates in the database
        foreach (var item in normalizedRequests)
        {
            bool exists = await _seatRepository.ExistsAsync(
                screenId,
                item.RowName!,
                item.Request.SeatNumber);

            if (exists)
            {
                throw new ArgumentException(
                    $"Seat {item.RowName}{item.Request.SeatNumber} already exists in this screen.");
            }
        }

        var seats = normalizedRequests.Select(item => new Seat
        {
            ScreenId = screenId,
            RowName = item.RowName!,
            SeatNumber = item.Request.SeatNumber,
            SeatType = item.Request.SeatType.Trim()
        }).ToList();

        await _seatRepository.AddRangeAsync(seats);

        return seats.Select(MapToResponse).ToList();
    }

    // Get seat by ID
    public async Task<SeatResponse?> GetSeatByIdAsync(int seatId)
    {
        if (seatId <= 0)
        {
            throw new ArgumentException("Invalid seat ID.");
        }

        var seat = await _seatRepository.GetByIdAsync(seatId);

        return seat == null ? null : MapToResponse(seat);
    }

    // Get all seats
    public async Task<List<SeatResponse>> GetAllSeatsAsync()
    {
        var seats = await _seatRepository.GetAllAsync();

        return seats.Select(MapToResponse).ToList();
    }

    // Get seats by screen ID
    public async Task<List<SeatResponse>> GetSeatsByScreenIdAsync(
        int screenId)
    {
        if (screenId <= 0)
        {
            throw new ArgumentException("Invalid screen ID.");
        }

        await ValidateScreenAsync(screenId);

        var seats = await _seatRepository.GetByScreenIdAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    // Update seat
    public async Task<SeatResponse?> UpdateSeatAsync(
        int seatId,
        UpdateSeatRequest request)
    {
        if (seatId <= 0)
        {
            throw new ArgumentException("Invalid seat ID.");
        }

        ValidateSeatDetails(
            request.RowName,
            request.SeatNumber,
            request.SeatType);

        var seat = await _seatRepository.GetByIdAsync(seatId);

        if (seat == null)
        {
            return null;
        }

        string normalizedRowName =
            request.RowName.Trim().ToUpperInvariant();

        bool exists = await _seatRepository.ExistsAsync(
            seat.ScreenId,
            normalizedRowName,
            request.SeatNumber,
            seatId);

        if (exists)
        {
            throw new ArgumentException(
                "Another seat with this row and seat number already exists in this screen.");
        }

        seat.RowName = normalizedRowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType.Trim();

        await _seatRepository.UpdateAsync(seat);

        return MapToResponse(seat);
    }

    // Delete seat
    public async Task<bool> DeleteSeatAsync(int seatId)
    {
        if (seatId <= 0)
        {
            throw new ArgumentException("Invalid seat ID.");
        }

        var seat = await _seatRepository.GetByIdAsync(seatId);

        if (seat == null)
        {
            return false;
        }

        return await _seatRepository.DeleteAsync(seatId);
    }

    // Validate screen
    private async Task ValidateScreenAsync(int screenId)
    {
        if (screenId <= 0)
        {
            throw new ArgumentException("Invalid screen ID.");
        }

        var screen = await _screenRepository.GetByIdAsync(screenId);

        if (screen == null)
        {
            throw new ArgumentException(
                $"Screen with ID {screenId} does not exist.");
        }
    }

    // Validate seat details
    private static void ValidateSeatDetails(
        string? rowName,
        int seatNumber,
        string? seatType)
    {
        if (string.IsNullOrWhiteSpace(rowName))
        {
            throw new ArgumentException("Row name is required.");
        }

        if (rowName.Trim().Length > 5)
        {
            throw new ArgumentException(
                "Row name cannot exceed 5 characters.");
        }

        if (seatNumber <= 0)
        {
            throw new ArgumentException(
                "Seat number must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(seatType))
        {
            throw new ArgumentException("Seat type is required.");
        }

        if (seatType.Trim().Length > 30)
        {
            throw new ArgumentException(
                "Seat type cannot exceed 30 characters.");
        }
    }

    // Map entity to response DTO
    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatType = seat.SeatType
        };
    }
}