using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("Movies/[action]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        private List<string> _allowdExtension = new List<string> { "jpg", "png" };
        private int _allowdMaxPosterSize = 2097152;


        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _context.Movies
                .OrderByDescending(x=>x.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieGenreDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    StoryLine = m.StoryLine,
                    Title = m.Title,
                    Year = m.Year
                })
                .ToListAsync();
            if (movies == null)
                return NotFound("Not Found Data");
            return Ok(movies);
        }
        [HttpGet]
        public async Task<IActionResult> GetByGenreId(byte genreId)
        {
            // repeated
            var movies = await _context.Movies
                .Where(m=> m.GenreId == genreId)
               .OrderByDescending(x => x.Rate)
               .Include(m => m.Genre)
               .Select(m => new MovieGenreDetailsDto
               {
                   Id = m.Id,
                   GenreId = m.GenreId,
                   GenreName = m.Genre.Name,
                   Poster = m.Poster,
                   StoryLine = m.StoryLine,
                   Title = m.Title,
                   Year = m.Year
               })
               .ToListAsync();
            if (movies == null)
                return NotFound("Not Found Data");
            return Ok(movies);
        }

        [HttpGet]
        public async Task<IActionResult> GetById([FromBody]int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).FirstOrDefaultAsync(g => g.Id == id);

            if (movie == null)
                return NotFound($"Not Found Movie with ID = {id}");

            var Dto = new MovieGenreDetailsDto
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                StoryLine = movie.StoryLine,
                Title = movie.Title,
                Year = movie.Year
            };
            return Ok(Dto);
        }
        [HttpPost]
        public async  Task<IActionResult> AddMovie([FromForm] MovieAddDto dto)
        {
            // check for Poster extension
            if (!_allowdExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("only .png and .jpg are allowed images");
            // check for large Poster size
            if(dto.Poster.Length > _allowdMaxPosterSize)
                return BadRequest("Max Size allowed for Poster 2 MB");
            // check for genre if it exist
            var isValidGenre = await _context.Genres.AnyAsync(g => g.GenreId == dto.GenreId);
            if(!isValidGenre)
                return BadRequest("Invalid Genre ID!");


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

        [HttpPut]
        public async Task<IActionResult> EditMovie(int id,[FromBody] MovieUpdateDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return BadRequest($"Not Found Movie with ID = {id}");

            // check for genre if it exist
            var isValidGenre = await _context.Genres.AnyAsync(g => g.GenreId == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID!");

            // Poster (Movie Image)
            if(dto.Poster != null)
            {
                // check for Poster extension
                if (!_allowdExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("only .png and .jpg are allowed images");
                // check for large Poster size
                if (dto.Poster.Length > _allowdMaxPosterSize)
                    return BadRequest("Max Size allowed for Poster 2 MB");

                // convert IFormFile to array of bytes
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }
            movie.GenreId = dto.GenreId;
            movie.Title = dto.Title;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.Year = dto.Year;

            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return BadRequest($"Not Found Movie with ID = {id}");

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
    }
}
