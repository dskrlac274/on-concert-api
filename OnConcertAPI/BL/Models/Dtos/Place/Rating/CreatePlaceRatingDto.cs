using OnConcert.BL.Models.Dtos.Event.Rating;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Place.Rating
{
    public class CreatePlaceRatingDto : GenericRatingDto
    {
        [JsonIgnore]
        public int PlaceId { get; set; }
        [JsonIgnore]
        public int? BandId { get; set; }
        [JsonIgnore]
        public int? VisitorId { get; set; }
    }
}