using OnConcert.BL.Models.Enums;

namespace OnConcert.BL.Models.Dtos.User
{
    public class AllUserDetailsDto : GenericUserDetailsDto
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }

        // Bands
        public string Discography { get; set; } = string.Empty;

        // Organizers
        public string Oib { get; set; } = string.Empty;

        // Places
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}