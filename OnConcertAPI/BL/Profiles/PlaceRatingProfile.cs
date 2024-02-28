using AutoMapper;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.Place.Rating;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class PlaceRatingProfile : Profile
    {
        public PlaceRatingProfile()
        {
            CreateMap<PlaceDetailsDto, Place>();
            CreateMap<Place, PlaceResponseDto>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.User, dest));

            CreateMap<CreatePlaceRatingDto, Rating>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score <= 0 ? 1 : (src.Score > 5 ? 5 : src.Score)));
            CreateMap<CreatePlaceRatingDto, PlaceRating>();
            CreateMap<PlaceRating, PlaceRatingResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RatingId));
            CreateMap<PlaceRating, GetPlaceRatingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RatingId));
        }
    }
}