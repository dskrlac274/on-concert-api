using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Band
{
    public class RegisterBandDto : RegisterUserDto
    {
        public string Discography { get; set; } = string.Empty;
    }
}