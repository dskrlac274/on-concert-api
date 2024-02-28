using AutoMapper;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Dtos.Visitor;
using OnConcert.DAL.Entities;

namespace OnConcert.BL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<LoginUserBaseDto, LoginUserDto>();
            CreateMap<RegisterPlaceDto, Place>();
            CreateMap<RegisterBandDto, Band>();
            CreateMap<RegisterOrganizerDto, Organizer>();
            CreateMap<RegisterPlaceDto, User>();
            CreateMap<RegisterBandDto, User>();
            CreateMap<RegisterOrganizerDto, User>();

            CreateMap<User, UserResponseDto>();
            CreateMap<User, BandDetailsDto>()
                .ForMember(dest => dest.Id, opts => opts.Ignore());
            CreateMap<User, OrganizerDetailsDto>()
                .ForMember(dest => dest.Id, opts => opts.Ignore());
            CreateMap<User, PlaceDetailsDto>()
                .ForMember(dest => dest.Id, opts => opts.Ignore());
            CreateMap<Band, BandDetailsDto>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.User, dest));
            CreateMap<Organizer, OrganizerDetailsDto>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.User, dest));
            CreateMap<Place, PlaceDetailsDto>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.User, dest));

            CreateMap<AllUserDetailsDto, BandDetailsDto>();
            CreateMap<AllUserDetailsDto, OrganizerDetailsDto>();
            CreateMap<AllUserDetailsDto, PlaceDetailsDto>();
            CreateMap<AllUserDetailsDto, GetUserDto>();

            CreateMap<BandDetailsDto, Band>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );
            CreateMap<BandDetailsDto, User>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );
            CreateMap<BandDetailsDto, BandResponseDto>();

            CreateMap<OrganizerDetailsDto, Organizer>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );
            CreateMap<OrganizerDetailsDto, User>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );

            CreateMap<PlaceDetailsDto, Place>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForMember(dest => dest.Capacity,
                    opts => opts.PreCondition(src => src.Capacity >= 1)
                )
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );
            CreateMap<PlaceDetailsDto, User>()
                .ForMember(dest => dest.Id, opts => opts.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((_, _, srcMember) => !string.IsNullOrWhiteSpace(srcMember.ToString()))
                );

            CreateMap<VisitorDetailsDto, Visitor>();
            CreateMap<Visitor, VisitorDetailsDto>();
        }
    }
}