using AutoMapper;
using OnConcert.BL.Models.Dtos.Event.Message;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class EventMessageProfile : Profile
    {
        public EventMessageProfile()
        {
            CreateMap<EventMessageRequestDto, Message>()
                .ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.OrganizerId == default ? (int?)null : src.OrganizerId))
                .ForMember(dest => dest.BandId, opt => opt.MapFrom(src => src.BandId == default ? (int?)null : src.BandId));
            CreateMap<Message, EventMessageResponseDto>();
        }
    }
}