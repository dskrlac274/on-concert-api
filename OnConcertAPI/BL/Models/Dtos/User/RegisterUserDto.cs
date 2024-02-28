namespace OnConcert.BL.Models.Dtos.User
{
    public class RegisterUserDto : GenericUserDetailsDto
    {
        public string Password { get; set; } = string.Empty;
    }
}