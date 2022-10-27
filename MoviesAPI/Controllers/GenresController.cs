using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly AppDbContext _Context;
        public GenresController(AppDbContext appDbContext)
        {
            _Context = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var geners = await _Context.Genres.OrderBy(g => g.Name).ToListAsync();
            if (geners == null)
                return NotFound("Not Found Data");

            return Ok(geners);
        }

        [HttpGet ("GetGenrebyID")]
        public async Task<IActionResult> GetGenrebyID(int id)
        {
            var genre = await _Context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null)
                return NotFound($"Not Found Genre with ID = {id}");


            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _Context.Genres.AddAsync(genre);
            _Context.SaveChanges();

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id,[FromBody]GenreDto dto)
        {
            var genre = await _Context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);

            if (genre == null)
                return NotFound($"Not Found Genre with ID = {id}");


            genre.Name = dto.Name;
            _Context.SaveChanges();

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _Context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);
            if(genre == null)
                return NotFound($"Not Found Genre with ID = {id}");

            _Context.Genres.Remove(genre);
            return Ok(genre);

        }


    }
}