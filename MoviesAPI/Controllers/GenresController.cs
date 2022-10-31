using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("Genres/[action]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreServices _genreService;
        public GenresController(IGenreServices genreServices)
        {
            _genreService = genreServices;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var geners = await _genreService.GetAll();

            if (geners == null)
                return NotFound("Not Found Data");

            return Ok(geners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGenrebyID(byte id)
        {
            var genre = await _genreService.GetById(id);

            if (genre == null)
                return NotFound($"Not Found Genre with ID = {id}");


            return Ok(genre);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            await _genreService.Add(genre);

            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genreService.GetById(id);

            if (genre == null)
                return NotFound($"Not Found Genre with ID = {id}");


            genre.Name = dto.Name;
            _genreService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(byte id)
        {
            var genre = await _genreService.GetById(id);
            if (genre == null)
                return NotFound($"Not Found Genre with ID = {id}");

            _genreService.Delete(genre);
            return Ok(genre);

        }


    }
}