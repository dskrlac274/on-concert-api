using AutoMapper;
using OnConcert.BL.Models.Dtos.Band.Rating;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class BandRatingProfile : Profile
    {
        public BandRatingProfile()
        {
            CreateMap<CreateBandRatingDto, Rating>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score <= 0 ? 1 : (src.Score > 5 ? 5 : src.Score)));
            CreateMap<CreateBandRatingDto, BandRating>();
            CreateMap<BandRating, BandRatingResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RatingId));
            CreateMap<Rating, GetBandRatingDto>();
            CreateMap<BandRating, GetBandRatingDto>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.Rating, dest));
        }
    }
}