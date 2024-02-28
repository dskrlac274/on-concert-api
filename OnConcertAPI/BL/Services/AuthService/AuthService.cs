using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Dtos.Visitor;
using OnConcert.BL.Models.Enums;
using OnConcert.Core.Helpers;
using OnConcert.DAL.Entities;
using OnConcert.DAL.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace OnConcert.BL.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Organizer> _organizerRepository;
        private readonly IRepository<Band> _bandRepository;
        private readonly IRepository<Place> _placeRepository;
        private readonly IRepository<Visitor> _visitorRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IMapper _mapper;

        public AuthService(
            IRepository<User> userRepository,
            IRepository<Organizer> organizerRepository,
            IRepository<Band> bandRepository,
            IRepository<Place> placeRepository,
            IRepository<Visitor> visitorRepository,
            IJwtHelper jwtHelper,
            IMapper mapper
        )
        {
            _userRepository = userRepository;
            _organizerRepository = organizerRepository;
            _bandRepository = bandRepository;
            _placeRepository = placeRepository;
            _visitorRepository = visitorRepository;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<object>> Register(RegisterPlaceDto registerPlaceDto)
        {
            var response = await CreateUserExistsResponse(registerPlaceDto.Email);

            if (!response.Success) return response;

            var user = await CreateUser(registerPlaceDto);
            var place = _mapper.Map<Place>(registerPlaceDto);
            place.UserId = user.Id;

            _placeRepository.Add(place);
            await _placeRepository.Save();

            return response;
        }

        public async Task<ServiceResponse<object>> Register(RegisterBandDto registerBandDto)
        {
            var response = await CreateUserExistsResponse(registerBandDto.Email);

            if (!response.Success) return response;

            var user = await CreateUser(registerBandDto);
            var band = _mapper.Map<Band>(registerBandDto);
            band.UserId = user.Id;

            _bandRepository.Add(band);
            await _bandRepository.Save();

            return response;
        }

        public async Task<ServiceResponse<object>> Register(RegisterOrganizerDto registerOrganizerDto)
        {
            var response = await CreateUserExistsResponse(registerOrganizerDto.Email);

            if (!response.Success) return response;

            if (await CheckOrganizerExistsByOib(registerOrganizerDto.Oib))
                return ServiceResponseBuilder.CreateErrorResponse<object>(
                    "Organizer with the same OIB already exists.");

            var user = await CreateUser(registerOrganizerDto);
            var organizer = _mapper.Map<Organizer>(registerOrganizerDto);
            organizer.UserId = user.Id;

            _organizerRepository.Add(organizer);
            await _placeRepository.Save();

            return response;
        }

        public async Task<ServiceResponse<LoginUserResponseDto>> Login(LoginUserDto loginDto)
        {
            var errorResponse = ServiceResponseBuilder.CreateErrorResponse<LoginUserResponseDto>("User not found.");

            var user = await FindUserByEmail(loginDto.Email);

            if (user is null) return errorResponse;

            var userRoleId = await FindUserIdByRole(user.Id, loginDto.Role);

            if (userRoleId is null) return errorResponse;

            if (!PasswordHelper.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                return ServiceResponseBuilder.CreateErrorResponse<LoginUserResponseDto>("Wrong password.");

            return ServiceResponseBuilder.CreateSuccessResponse(new LoginUserResponseDto
            {
                Jwt = _jwtHelper.CreateJwt(userRoleId.Value, loginDto.Role.ToString())
            }); 
        }

        public async Task<ServiceResponse<LoginUserResponseDto>> Login(VisitorDto loginDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = (JwtSecurityToken)handler.ReadToken(loginDto.Token);

            if (jsonToken == null)
                return ServiceResponseBuilder.CreateErrorResponse<LoginUserResponseDto>("Invalid token.");

            var visitorEmail = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var visitorName = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;

            if (visitorEmail == null)
                return ServiceResponseBuilder.CreateErrorResponse<LoginUserResponseDto>("Email can't be empty.");

            if (visitorName == null)
                return ServiceResponseBuilder.CreateErrorResponse<LoginUserResponseDto>("Name can't be empty.");

            var visitor = await FindVisitorByEmail(visitorEmail);
            visitor ??= await CreateVisitor(
                new VisitorDetailsDto
                {
                    Name = visitorName,
                    Email = visitorEmail
                }
            );

            return ServiceResponseBuilder.CreateSuccessResponse(
                new LoginUserResponseDto
                {
                    Jwt = _jwtHelper.CreateJwt(visitor.Id, UserRole.Visitor.ToString())
                }
            );
        }

        private async Task<User> CreateUser(RegisterUserDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);

            PasswordHelper.CreatePasswordHash(registerDto.Password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userRepository.Add(user);
            await _userRepository.Save();

            return await _userRepository.GetAll().FirstAsync(u => u.Email.Equals(registerDto.Email));
        }

        private async Task<ServiceResponse<object>> CreateUserExistsResponse(string email)
        {
            if (await CheckUserExists(email))            
                return ServiceResponseBuilder.CreateErrorResponse<object>(
                   "User already exists.");

            return ServiceResponseBuilder.CreateSuccessResponse<object>(string.Empty);
        }

        private Task<bool> CheckOrganizerExistsByOib(string oib) =>
            _organizerRepository.GetAll().AnyAsync(o => o.Oib.ToLower().Equals(oib.ToLower()));

        private Task<bool> CheckUserExists(string email) =>
            _userRepository.GetAll().AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private Task<User?> FindUserByEmail(string email) =>
            _userRepository.GetAll().FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private async Task<int?> FindUserIdByRole(int genericId, UserRole role) =>
            role switch
            {
                UserRole.Organizer => (await _organizerRepository.GetAll()
                    .FirstOrDefaultAsync(o => o.UserId == genericId))?.Id,
                UserRole.Band => (await _bandRepository.GetAll()
                    .FirstOrDefaultAsync(b => b.UserId == genericId))?.Id,
                UserRole.Place => (await _placeRepository.GetAll()
                    .FirstOrDefaultAsync(p => p.UserId == genericId))?.Id,
                _ => null
            };

        private Task<Visitor?> FindVisitorByEmail(string email) =>
            _visitorRepository.GetAll().FirstOrDefaultAsync(u => u.Email.Equals(email));

        private async Task<Visitor> CreateVisitor(VisitorDetailsDto visitorDetailsDto)
        {
            var visitor = _mapper.Map<Visitor>(visitorDetailsDto);

            _visitorRepository.Add(visitor);
            await _visitorRepository.Save();

            return await _visitorRepository.GetAll().FirstAsync(v => v.Email.Equals(visitorDetailsDto.Email));
        }
    }
}