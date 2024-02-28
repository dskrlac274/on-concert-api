using AutoMapper;
using OnConcert.BL.Models.Dtos.Event.Application;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class EventApplicationProfile : Profile
    {
        public EventApplicationProfile()
        {
            CreateMap<EventApplication, EventApplicationResponseDto>();
        }
    }
}