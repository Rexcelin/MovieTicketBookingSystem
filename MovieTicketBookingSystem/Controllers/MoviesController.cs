using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBookingSystem.Constants;
using MovieTicketBookingSystem.DTOs.Movies;
using MovieTicketBookingSystem.Services.Interfaces;

namespace MovieTicketBookingSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    // POST: api/movies
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateMovie(
        [FromBody] CreateMovieRequest request)
    {
        try
        {
            var movie = await _movieService.CreateMovieAsync(request);

            return CreatedAtAction(
                nameof(GetMovieById),
                new { id = movie.MovieId },
                movie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/movies?pageNumber=1&pageSize=10&title=leo
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet]
    public async Task<IActionResult> GetMovies(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null)
    {
        try
        {
            var result = await _movieService.GetMoviesAsync(
                pageNumber,
                pageSize,
                title);

            return Ok(new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(
                    result.TotalCount / (double)pageSize),
                Movies = result.Movies
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET: api/movies/1
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMovieById(int id)
    {
        try
        {
            var movie = await _movieService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

            return Ok(movie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/movies/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMovie(
        int id,
        [FromBody] UpdateMovieRequest request)
    {
        try
        {
            var movie = await _movieService.UpdateMovieAsync(id, request);

            if (movie == null)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

            return Ok(movie);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/movies/1
    // Admin only
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            bool deleted = await _movieService.DeleteMovieAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Movie with ID {id} was not found."
                });
            }

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
                message = "This movie cannot be deleted because it is associated with existing shows or bookings."
            });
        }
    }

    // GET: api/movies/search?title=leo
    // Admin and Customer
    [Authorize(Policy = "AdminOrCustomer")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(
        [FromQuery] string title)
    {
        try
        {
            var movies = await _movieService.SearchMoviesAsync(title);

            return Ok(movies);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}