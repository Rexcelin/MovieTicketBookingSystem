using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.DTOs.Seats;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatsController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    // POST: api/seats
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateSeat(
        [FromBody] CreateSeatRequest request)
    {
        try
        {
            var seat = await _seatService.CreateSeatAsync(request);

            return CreatedAtAction(
                nameof(GetSeatById),
                new { id = seat.SeatId },
                seat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to create the seat. Check for duplicate seat details."
            });
        }
    }

    // POST: api/seats/bulk
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateSeatsBulk(
        [FromBody] List<CreateSeatRequest> requests)
    {
        try
        {
            var seats = await _seatService.CreateSeatsBulkAsync(requests);

            return StatusCode(StatusCodes.Status201Created, seats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to create seats. Check for duplicate seat details."
            });
        }
    }

    // GET: api/seats
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet]
    public async Task<IActionResult> GetAllSeats()
    {
        var seats = await _seatService.GetAllSeatsAsync();

        return Ok(seats);
    }

    // GET: api/seats/5
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetSeatById(int id)
    {
        try
        {
            var seat = await _seatService.GetSeatByIdAsync(id);

            if (seat == null)
                return NotFound(new { message = "Seat not found." });

            return Ok(seat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/seats/screen/1
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetSeatsByScreenId(int screenId)
    {
        try
        {
            var seats = await _seatService.GetSeatsByScreenIdAsync(screenId);

            return Ok(seats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/seats/5
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSeat(
        int id,
        [FromBody] UpdateSeatRequest request)
    {
        try
        {
            var seat = await _seatService.UpdateSeatAsync(id, request);

            if (seat == null)
                return NotFound(new { message = "Seat not found." });

            return Ok(seat);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to update the seat. Check for duplicate seat details."
            });
        }
    }

    // DELETE: api/seats/5
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteSeat(int id)
    {
        try
        {
            bool deleted = await _seatService.DeleteSeatAsync(id);

            if (!deleted)
                return NotFound(new { message = "Seat not found." });

            return NoContent();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "This seat cannot be deleted because it is associated with existing show or booking records."
            });
        }
    }
}