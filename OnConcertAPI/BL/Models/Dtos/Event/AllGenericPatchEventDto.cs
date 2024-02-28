using OnConcert.BL.Models.Enums;
using OnConcert.DAL.Enums;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Event
{
    public class AllGenericPatchEventDto : GenericEventDetailsDto
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        [JsonIgnore]
        public int OrganizerId { get; set; }
        public int PlaceId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlaceReservationStatus PlaceReservationStatus { get; set; } = PlaceReservationStatus.Requested;
    }
}