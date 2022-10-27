using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            if (movies == null)
                return NotFound("Not Found Data");
            return Ok(movies);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromBody]int id)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(g => g.Id == id);
            if (movie == null)
                return NotFound($"Not Found Movie with ID = {id}");
            return Ok(movie);
        }
        [HttpPost]
        public async  Task<IActionResult> AddMovie([FromForm] MovieDto dto)
        {
            // convert IFormFile to array of bytes
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoryLine = dto.StoryLine,
                Year = dto.Year

            };

            await _context.AddAsync(movie);
             _context.SaveChanges();
            return Ok(movie);
        }
    }
}
