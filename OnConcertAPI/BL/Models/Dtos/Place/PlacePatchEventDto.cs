using OnConcert.DAL.Enums;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Place
{
    public class PlacePatchEventDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PlaceReservationStatus PlaceReservationStatus { get; set; } = PlaceReservationStatus.Requested;
    }
}