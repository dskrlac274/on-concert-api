namespace OnConcert.BL.Models.Dtos.User
{
    public class GenericUserDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string WebUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}