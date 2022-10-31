using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [Route("Movies/[action]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IGenreServices _genreServices;
        private readonly IMapper _mapper;
        private List<string> _allowdExtension = new List<string> { ".jpg", ".png" };
        private int _allowdMaxPosterSize = 2097152;


        public MoviesController(IMovieService movieService, IGenreServices genreServices, IMapper mapper)
        {
            _movieService = movieService;
            _genreServices = genreServices;
            _mapper = mapper;
        }


        // Get All Movies
        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieService.GetAll();
           
            var dto = _mapper.Map<IEnumerable<MovieGenreDetailsDto>>(movies);

            return Ok(dto);
        }


        [HttpGet]
        // Get Movies related To speacific Genre
        public async Task<IActionResult> GetByGenreId(byte genreId)
        {
            var movies = await _movieService.GetAll(genreId);

            // using auto Mapper
            var dto = _mapper.Map<IEnumerable<MovieGenreDetailsDto>>(movies);

            return Ok(dto);
        }


        // Get speacific Movie by Id
        [HttpGet]
        public async Task<IActionResult> GetById([FromBody]int id)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return NotFound("Not Found");

            var dto = _mapper.Map<MovieGenreDetailsDto>(movie);

            return Ok(dto);
        }


        // Add New Movie
        [HttpPost]
        public async Task<IActionResult> AddMovie([FromForm] MovieAddDto dto)
        {
            // check for Poster extension
            if (!_allowdExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("only .png and .jpg are allowed images");
            // check for large Poster size
            if(dto.Poster.Length > _allowdMaxPosterSize)
                return BadRequest("Max Size allowed for Poster 2 MB");
            // check for genre if it exist
            var isValidGenre = await _genreServices.isValidGenre(dto.GenreId);
            if(!isValidGenre)
                return BadRequest("Invalid Genre ID!");


            // convert IFormFile to array of bytes
            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);
            // mapped using auto mapper
            var movie = _mapper.Map<Movie>(dto);
            // mapped manually
            movie.Poster = dataStream.ToArray();

             _movieService.Add(movie);
            
            return Ok(movie);
        }

        // Edit New Movie
        [HttpPut]
        public async Task<IActionResult> EditMovie(int id,[FromForm] MovieUpdateDto dto)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return BadRequest($"Not Found Movie with ID = {id}");

            // check for genre if it exist
            var isValidGenre = await _genreServices.isValidGenre(dto.GenreId);
            if (!isValidGenre)
                return BadRequest("Invalid Genre ID!");

            // Poster (Movie Image)
            if (dto.Poster != null)
            {
                // check for Poster extension
                if (!_allowdExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("only .png and .jpg are allowed images");
                // check for large Poster size
                if (dto.Poster.Length > _allowdMaxPosterSize)
                    return BadRequest("Max Size allowed for Poster 2 MB");


                using var dataStream = new MemoryStream();
                // convert IFormFile to array of bytes
                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();

            }
            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoryLine = dto.StoryLine;
            movie.Rate = dto.Rate;


            _movieService.Update(movie);
            return Ok(movie);
        }


        // Delete Movie
        [HttpDelete]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return BadRequest($"Not Found Movie with ID = {id}");

            _movieService.Delete(movie);

            return Ok(movie);
        }
    }
}
