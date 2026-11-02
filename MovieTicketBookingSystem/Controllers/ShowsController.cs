
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.DTOs.Shows;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShowsController : ControllerBase
{
    private readonly IShowService _showService;

    public ShowsController(IShowService showService)
    {
        _showService = showService;
    }

    // POST: api/shows
    [HttpPost]
    public async Task<IActionResult> CreateShow(
        [FromBody] CreateShowRequest request)
    {
        try
        {
            var show = await _showService.CreateShowAsync(request);

            return CreatedAtAction(
                nameof(GetShowById),
                new { id = show.ShowId },
                show);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to create the show due to a database constraint."
            });
        }
    }

    // GET: api/shows
    [HttpGet]
    public async Task<IActionResult> GetAllShows()
    {
        var shows = await _showService.GetAllShowsAsync();

        return Ok(shows);
    }

    // GET: api/shows/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShowById(int id)
    {
        try
        {
            var show = await _showService.GetShowByIdAsync(id);

            if (show == null)
                return NotFound(new { message = "Show not found." });

            return Ok(show);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/shows/movie/1
    [HttpGet("movie/{movieId:int}")]
    public async Task<IActionResult> GetShowsByMovieId(int movieId)
    {
        try
        {
            var shows = await _showService.GetShowsByMovieIdAsync(movieId);

            return Ok(shows);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/shows/screen/1
    [HttpGet("screen/{screenId:int}")]
    public async Task<IActionResult> GetShowsByScreenId(int screenId)
    {
        try
        {
            var shows = await _showService.GetShowsByScreenIdAsync(screenId);

            return Ok(shows);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/shows/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateShow(
        int id,
        [FromBody] UpdateShowRequest request)
    {
        try
        {
            var show = await _showService.UpdateShowAsync(id, request);

            if (show == null)
                return NotFound(new { message = "Show not found." });

            return Ok(show);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to update the show due to a database constraint."
            });
        }
    }

    // DELETE: api/shows/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteShow(int id)
    {
        try
        {
            bool deleted = await _showService.DeleteShowAsync(id);

            if (!deleted)
                return NotFound(new { message = "Show not found." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Unable to delete the show because dependent records exist."
            });
        }
    }
    // GET: api/shows/1/seats
    [HttpGet("{showId:int}/seats")]
    public async Task<IActionResult> GetShowSeats(int showId)
    {
        try
        {
            var seats = await _showService.GetShowSeatsAsync(showId);

            return Ok(seats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
