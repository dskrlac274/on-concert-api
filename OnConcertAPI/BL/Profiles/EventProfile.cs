using AutoMapper;
using OnConcert.BL.Models.Dtos.Event;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<CreateEventDto, Event>();
            CreateMap<Event, EventResponseDto>();

            CreateMap<AllGenericPatchEventDto, OrganizerPatchEventDto>();
            CreateMap<AllGenericPatchEventDto, PlacePatchEventDto>();

            CreateMap<OrganizerPatchEventDto, Event>()
                .ForMember(dest => dest.OrganizerId, opts => opts.Ignore())
                .ForMember(dest => dest.PlaceId, opts => opts.Ignore())
                .ForMember(dest => dest.PlaceReservationStatus, opts => opts.Ignore())
                .ForMember(dest => dest.Name,
                    opts => opts.Condition(src => !string.IsNullOrWhiteSpace(src.Name)))
                .ForMember(dest => dest.Description,
                    opts => opts.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
                .ForMember(dest => dest.DateFrom,
                    opts => opts.Condition(src => src.DateFrom != DateTime.MinValue))
                .ForMember(dest => dest.DateTo,
                    opts => opts.Condition(src => src.DateTo != DateTime.MinValue));
            CreateMap<PlacePatchEventDto, Event>()
                .ForMember(dest => dest.PlaceId, opts => opts.Ignore());
            CreateMap<PlacePatchEventDto, Event>();
        }
    }
}