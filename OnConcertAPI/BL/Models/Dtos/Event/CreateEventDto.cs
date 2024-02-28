using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Event
{
    public class CreateEventDto : GenericEventDetailsDto
    {
        [JsonIgnore]
        public int OrganizerId { get; set; }
        public int PlaceId { get; set; }
    }
}