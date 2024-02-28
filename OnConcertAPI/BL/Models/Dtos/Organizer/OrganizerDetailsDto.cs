using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Organizer
{
    public class OrganizerDetailsDto : GenericUserDetailsDto
    {
        public int Id { get; set; }
        public string Oib { get; set; } = string.Empty;
    }
}