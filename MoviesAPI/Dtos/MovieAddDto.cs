namespace MoviesAPI.Dtos
{
    public class MovieAddDto : MovieBaseDto
    {
        public IFormFile Poster { get; set; }

    }
}
