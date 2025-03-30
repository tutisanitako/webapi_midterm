using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DataEntity
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Showtimes, ShowtimeDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movies.Title))
                .ForMember(dest => dest.HallName, opt => opt.MapFrom(src => src.Halls.HallName));

            CreateMap<ShowtimeDto, Showtimes>();
        }
    }
}