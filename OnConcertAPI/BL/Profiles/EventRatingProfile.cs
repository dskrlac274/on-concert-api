using AutoMapper;
using OnConcert.BL.Models.Dtos.Event.Rating;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class EventRatingProfile : Profile
    {
        public EventRatingProfile()
        {
            CreateMap<CreateEventRatingDto, Rating>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score <= 0 ? 1 : (src.Score > 5 ? 5 : src.Score)));
            CreateMap<CreateEventRatingDto, EventRating>();
            CreateMap<EventRating, EventRatingResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RatingId));
            CreateMap<EventRating, GetEventRatingDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RatingId));
        }
    }
}