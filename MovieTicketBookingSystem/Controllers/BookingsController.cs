
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DTOs.Bookings;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(request);

            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = booking.BookingId },
                booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/bookings
    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookingsAsync();

        return Ok(bookings);
    }

    // GET: api/bookings/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return Ok(booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/bookings/1/cancel
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        try
        {
            bool cancelled = await _bookingService.CancelBookingAsync(id);

            if (!cancelled)
            {
                return NotFound(new
                {
                    message = "Booking not found."
                });
            }

            return Ok(new
            {
                message = "Booking cancelled successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
