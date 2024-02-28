using OnConcert.BL.Models.Dtos.Event.Rating;
using OnConcert.DAL.Enums;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Event
{
    public class EventResponseDto : GenericEventDetailsDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
        public int OrganizerId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlaceReservationStatus PlaceReservationStatus { get; set; }
        public List<GetEventRatingDto>? Ratings { get; set; }
    }
}