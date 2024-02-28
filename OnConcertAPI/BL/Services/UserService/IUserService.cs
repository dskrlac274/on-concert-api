using OnConcert.BL.Models;
using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<object>> GetCurrentUser(GetUserDto userDto);
        Task<ServiceResponse<object>> UpdateCurrentUser(AllUserDetailsDto userDetails);
        Task<ServiceResponse<List<UserResponseDto>>> GetAll();
    }
}