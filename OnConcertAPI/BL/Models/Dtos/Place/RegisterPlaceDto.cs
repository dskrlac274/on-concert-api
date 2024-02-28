using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Place
{
    public class RegisterPlaceDto : RegisterUserDto
    {
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}