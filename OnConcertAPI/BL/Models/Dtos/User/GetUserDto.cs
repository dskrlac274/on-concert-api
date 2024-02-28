using OnConcert.BL.Models.Enums;

namespace OnConcert.BL.Models.Dtos.User
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
    }
}