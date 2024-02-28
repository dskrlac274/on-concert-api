using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Event.Rating
{
    public class CreateEventRatingDto : GenericRatingDto
    {
        [JsonIgnore]
        public int EventId { get; set; }
        [JsonIgnore]
        public int BandId { get; set; }
    }
}