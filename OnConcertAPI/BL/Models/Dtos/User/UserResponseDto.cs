namespace OnConcert.BL.Models.Dtos.User
{
    public class UserResponseDto 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}