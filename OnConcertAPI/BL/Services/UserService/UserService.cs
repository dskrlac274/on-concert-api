using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Dtos.Visitor;
using OnConcert.BL.Models.Enums;
using OnConcert.BL.Services.BandService;
using OnConcert.BL.Services.OrganizerService;
using OnConcert.BL.Services.PlaceService;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;

namespace OnConcert.BL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IBandService _bandService;
        private readonly IOrganizerService _organizerService;
        private readonly IPlaceService _placeService;
        private readonly IReadOnlyRepository<Organizer> _organizerRepository;
        private readonly IReadOnlyRepository<Band> _bandRepository;
        private readonly IReadOnlyRepository<Place> _placeRepository;
        private readonly IReadOnlyRepository<Visitor> _visitorRepository;
        private readonly IReadOnlyRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public UserService(
            IBandService bandService,
            IOrganizerService organizerService,
            IPlaceService placeService,
            IReadOnlyRepository<Organizer> organizerRepository,
            IReadOnlyRepository<Band> bandRepository,
            IReadOnlyRepository<Place> placeRepository,
            IReadOnlyRepository<Visitor> visitorRepository,
            IReadOnlyRepository<User> userRepository,
            IMapper mapper
        )
        {
            _bandService = bandService;
            _organizerService = organizerService;
            _placeService = placeService;
            _organizerRepository = organizerRepository;
            _bandRepository = bandRepository;
            _placeRepository = placeRepository;
            _visitorRepository = visitorRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<object>> GetCurrentUser(GetUserDto userDto) =>
            new()
            {
                Data = userDto.Role switch
                {
                    UserRole.Organizer => _mapper.Map<OrganizerDetailsDto>(
                        await GetCurrentUser(_organizerRepository, userDto.Id)
                    ),
                    UserRole.Band => _mapper.Map<BandDetailsDto>(
                        await GetCurrentUser(_bandRepository, userDto.Id)
                    ),
                    UserRole.Place => _mapper.Map<PlaceDetailsDto>(
                        await GetCurrentUser(_placeRepository, userDto.Id)
                    ),
                    UserRole.Visitor => _mapper.Map<VisitorDetailsDto>(
                        await _visitorRepository.GetAll().FirstOrDefaultAsync(v => v.Id == userDto.Id)
                    ),
                    _ => null
                }
            };

        public async Task<ServiceResponse<object>> UpdateCurrentUser(AllUserDetailsDto userDto)
        {
            var response = userDto.Role switch
            {
                UserRole.Organizer => await _organizerService.Update(_mapper.Map<OrganizerDetailsDto>(userDto)),
                UserRole.Band => await _bandService.Update(_mapper.Map<BandDetailsDto>(userDto)),
                UserRole.Place => await _placeService.Update(_mapper.Map<PlaceDetailsDto>(userDto)),
                _ => EmptyServiceResponseBuilder.CreateErrorResponse("Unrecognized role.")
            };

            return response.Success
                ? await GetCurrentUser(_mapper.Map<GetUserDto>(userDto))
                : ServiceResponseBuilder.CreateErrorResponse<object>(response.Message);
        }

        public async Task<ServiceResponse<List<UserResponseDto>>> GetAll()
        {
            var allUsers = await _userRepository.GetAll().ToListAsync();

            var userResponseDto = _mapper.Map<List<UserResponseDto>>(allUsers);

            return ServiceResponseBuilder.CreateSuccessResponse(userResponseDto);
        }  

        private static Task<T?> GetCurrentUser<T>(IReadOnlyRepository<T> repo, int userId) where T : class =>
            repo
                .GetAll()
                .Include("User")
                .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == userId);
    }
}