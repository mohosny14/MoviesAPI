using AutoMapper;
using MoviesAPI.Dtos;

namespace MoviesAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieGenreDetailsDto>();

            CreateMap<MovieAddDto, Movie>()
                .ForMember(s => s.Poster, o => o.Ignore());

        }
    }
}
