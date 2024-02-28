using OnConcert.BL.Models.Enums;

namespace OnConcert.BL.Models.Dtos.User
{
    public class LoginUserDto : LoginUserBaseDto
    {
        public UserRole Role { get; set; }
    }
}