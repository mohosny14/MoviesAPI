namespace MoviesAPI.Dtos
{
    public class MovieUpdateDto :MovieBaseDto
    {
        public IFormFile? Poster { get; set; }

    }
}
