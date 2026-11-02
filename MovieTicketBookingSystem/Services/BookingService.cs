using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Models;
using MovieTicketBookingSystem.Repositories.Interfaces;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowRepository _showRepository;
    private readonly IShowSeatRepository _showSeatRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IShowRepository showRepository,
        IShowSeatRepository showSeatRepository)
    {
        _bookingRepository = bookingRepository;
        _showRepository = showRepository;
        _showSeatRepository = showSeatRepository;
    }

    public async Task<BookingResponse> CreateBookingAsync(
        CreateBookingRequest request,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Invalid user.");

        if (request.SeatIds == null || request.SeatIds.Count == 0)
            throw new InvalidOperationException(
                "Please select at least one seat.");

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
            throw new InvalidOperationException(
                "Duplicate seat IDs are not allowed.");

        var show = await _showRepository.GetByIdAsync(request.ShowId);

        if (show == null)
            throw new KeyNotFoundException("Show not found.");

        var showSeats = await _showSeatRepository
            .GetByShowIdAsync(request.ShowId);

        var selectedSeats = showSeats
            .Where(ss => request.SeatIds.Contains(ss.SeatId))
            .ToList();

        if (selectedSeats.Count != request.SeatIds.Count)
            throw new InvalidOperationException(
                "One or more selected seats do not belong to this show.");

        if (selectedSeats.Any(ss => ss.Status != "Available"))
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");

        decimal totalAmount =
            show.TicketPrice * selectedSeats.Count;

        var booking = new Booking
        {
            BookingReference = GenerateBookingReference(),
            ShowId = request.ShowId,
            UserId = userId,

            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,

            BookingDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = "Confirmed",

            BookingSeats = selectedSeats.Select(ss => new BookingSeat
            {
                SeatId = ss.SeatId,
                Price = show.TicketPrice
            }).ToList()
        };

        int bookingId = await _bookingRepository.CreateAsync(
            booking,
            selectedSeats);

        var savedBooking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (savedBooking == null)
            throw new InvalidOperationException(
                "Booking was created, but its details could not be retrieved.");

        return MapToResponse(savedBooking);
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(
        int bookingId,
        string? userId = null)
    {
        if (bookingId <= 0)
            throw new ArgumentException("Invalid booking ID.");

        var booking = string.IsNullOrWhiteSpace(userId)
            ? await _bookingRepository.GetByIdAsync(bookingId)
            : await _bookingRepository.GetByIdForUserAsync(
                bookingId, userId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<List<BookingResponse>> GetAllBookingsAsync(
        string? userId = null)
    {
        var bookings = string.IsNullOrWhiteSpace(userId)
            ? await _bookingRepository.GetAllAsync()
            : await _bookingRepository.GetByUserIdAsync(userId);

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<bool> CancelBookingAsync(
        int bookingId,
        string? userId = null)
    {
        if (bookingId <= 0)
            throw new ArgumentException("Invalid booking ID.");

        var booking = string.IsNullOrWhiteSpace(userId)
            ? await _bookingRepository.GetByIdAsync(bookingId)
            : await _bookingRepository.GetByIdForUserAsync(
                bookingId, userId);

        if (booking == null)
            return false;

        if (booking.Status == "Cancelled")
            throw new InvalidOperationException(
                "This booking is already cancelled.");

        var bookingSeatIds = booking.BookingSeats
            .Select(bs => bs.SeatId)
            .ToHashSet();

        var showSeats = await _showSeatRepository
            .GetByShowIdAsync(booking.ShowId);

        var seatsToRelease = showSeats
            .Where(ss => bookingSeatIds.Contains(ss.SeatId))
            .ToList();

        if (seatsToRelease.Count != bookingSeatIds.Count)
            throw new InvalidOperationException(
                "Booking seat records could not be matched with show seats.");

        await _bookingRepository.CancelAsync(
            booking,
            seatsToRelease);

        return true;
    }

    private static string GenerateBookingReference()
    {
        return $"MTB-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
               Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            ShowId = booking.ShowId,

            MovieTitle = booking.Show.Movie.Title,
            TheatreName = booking.Show.Screen.Theatre.Name,
            ScreenName = booking.Show.Screen.ScreenName,

            StartTime = booking.Show.StartTime,
            EndTime = booking.Show.EndTime,

            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,

            BookingDate = booking.BookingDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,

            Seats = booking.BookingSeats.Select(bs =>
                new BookingSeatResponse
                {
                    BookingSeatId = bs.BookingSeatId,
                    SeatId = bs.SeatId,
                    RowName = bs.Seat.RowName,
                    SeatNumber = bs.Seat.SeatNumber,
                    SeatType = bs.Seat.SeatType,
                    Price = bs.Price
                }).ToList()
        };
    }
}