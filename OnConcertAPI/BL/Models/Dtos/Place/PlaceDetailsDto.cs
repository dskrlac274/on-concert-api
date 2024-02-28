using OnConcert.BL.Models.Dtos.User;

namespace OnConcert.BL.Models.Dtos.Place
{
    public class PlaceDetailsDto : GenericUserDetailsDto
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}