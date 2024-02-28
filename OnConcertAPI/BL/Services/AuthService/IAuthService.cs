using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.Band;
using OnConcert.BL.Models.Dtos.Organizer;
using OnConcert.BL.Models.Dtos.Place;
using OnConcert.BL.Models.Dtos.User;
using OnConcert.BL.Models.Dtos.Visitor;

namespace OnConcert.BL.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<object>> Register(RegisterPlaceDto registerPlaceDto);
        Task<ServiceResponse<object>> Register(RegisterBandDto registerBandDto);
        Task<ServiceResponse<object>> Register(RegisterOrganizerDto registerOrganizerDto);
        Task<ServiceResponse<LoginUserResponseDto>> Login(LoginUserDto loginDto);
        Task<ServiceResponse<LoginUserResponseDto>> Login(VisitorDto loginDto);
    }
}