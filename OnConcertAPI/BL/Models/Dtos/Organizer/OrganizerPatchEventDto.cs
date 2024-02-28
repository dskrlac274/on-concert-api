using OnConcert.BL.Models.Dtos.Event;

namespace OnConcert.BL.Models.Dtos.Organizer
{
    public class OrganizerPatchEventDto : GenericEventDetailsDto
    {
        public int PlaceId { get; set; }
    }
}