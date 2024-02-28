using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Organizer
{
    public class RegisterOrganizerDto : RegisterUserDto
    {
        public string Oib { get; set; } = string.Empty;
    }
}