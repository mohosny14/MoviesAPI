using Microsoft.EntityFrameworkCore;
using MoviesAPI.Dtos;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext _Context;

        public MovieService(AppDbContext appDbContext)
        {
            _Context = appDbContext;
        }
        public async Task<Movie> Add(Movie movie)
        {
            await _Context.AddAsync(movie);
            _Context.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _Context.Remove(movie);
            _Context.SaveChanges();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)
        {
            var movies = await _Context.Movies
                .Where(m => m.GenreId == genreId || genreId == 0)
                .OrderByDescending(m => m.Rate)
                .Include(m => m.Genre)
                .ToListAsync();

            return movies;
        }


        public async Task<Movie> GetById(int id)
        {
            var movie = await _Context.Movies.Include(m => m.Genre).FirstOrDefaultAsync(g => g.Id == id);
          
            return movie;

        }

        public Movie Update(Movie movie)
        {
            _Context.Update(movie);
            _Context.SaveChanges();
            return movie;
        }
    }
}
