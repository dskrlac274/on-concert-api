using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Band
{
    public class BandDetailsDto : GenericUserDetailsDto
    {
        public int Id { get; set; }
        public string Discography { get; set; } = string.Empty;
    }
}