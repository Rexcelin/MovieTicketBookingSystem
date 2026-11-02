
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
        CreateBookingRequest request)
    {
        // Validate selected seats
        if (request.SeatIds == null || request.SeatIds.Count == 0)
        {
            throw new InvalidOperationException(
                "Please select at least one seat.");
        }

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
        {
            throw new InvalidOperationException(
                "Duplicate seat IDs are not allowed.");
        }

        // Check whether the show exists
        var show = await _showRepository.GetByIdAsync(request.ShowId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        // Retrieve all seat availability records for this show
        var showSeats = await _showSeatRepository
            .GetByShowIdAsync(request.ShowId);

        // Find the requested seats
        var selectedSeats = showSeats
            .Where(ss => request.SeatIds.Contains(ss.SeatId))
            .ToList();

        // Ensure every requested seat belongs to this show
        if (selectedSeats.Count != request.SeatIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected seats do not belong to this show.");
        }

        // Check seat availability
        if (selectedSeats.Any(ss => ss.Status != "Available"))
        {
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");
        }

        // Calculate total amount on the server
        decimal totalAmount =
            show.TicketPrice * selectedSeats.Count;

        // Create booking and its seat records
        var booking = new Booking
        {
            BookingReference = GenerateBookingReference(),
            ShowId = request.ShowId,
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

        // Save booking and update seat availability
        int bookingId = await _bookingRepository.CreateAsync(
            booking,
            selectedSeats);

        // Retrieve complete booking details
        var savedBooking = await _bookingRepository
            .GetByIdAsync(bookingId);

        if (savedBooking == null)
        {
            throw new InvalidOperationException(
                "Booking was created, but its details could not be retrieved.");
        }

        return MapToResponse(savedBooking);
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(
        int bookingId)
    {
        if (bookingId <= 0)
        {
            throw new ArgumentException("Invalid booking ID.");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<List<BookingResponse>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        if (bookingId <= 0)
        {
            throw new ArgumentException("Invalid booking ID.");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
        {
            return false;
        }

        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "This booking is already cancelled.");
        }

        var bookingSeatIds = booking.BookingSeats
            .Select(bs => bs.SeatId)
            .ToHashSet();

        var showSeats = await _showSeatRepository
            .GetByShowIdAsync(booking.ShowId);

        var seatsToRelease = showSeats
            .Where(ss => bookingSeatIds.Contains(ss.SeatId))
            .ToList();

        if (seatsToRelease.Count != bookingSeatIds.Count)
        {
            throw new InvalidOperationException(
                "Booking seat records could not be matched with show seats.");
        }

        await _bookingRepository.CancelAsync(booking, seatsToRelease);

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
