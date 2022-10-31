using Microsoft.EntityFrameworkCore;
using MoviesAPI.Models;

namespace MoviesAPI.Services
{
    public class GenreServices : IGenreServices
    {
        private readonly AppDbContext _context;

        public GenreServices(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<Genre> Add(Genre genre)
        {
           await _context.Genres.AddAsync(genre);
            _context.SaveChanges();

            return genre;

        }

        public Genre Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return genre;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();

            return genres;

            // refactor       
           //  return await _context.Genres.OrderBy(g => g.Name).ToListAsync();

        }

        public async Task<Genre> GetById(byte id)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.GenreId == id);
        }

        public  Task<bool> isValidGenre(byte id)
        {
            return  _context.Genres.AnyAsync(g => g.GenreId == id);
        }

        public Genre Update(Genre genre)
        {
             _context.Update(genre);
            _context.SaveChanges();

            return genre;
        }
    }
}
