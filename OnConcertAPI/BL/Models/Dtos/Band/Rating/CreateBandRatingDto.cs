using OnConcert.BL.Models.Dtos.Event.Rating;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Band.Rating
{
    public class CreateBandRatingDto : GenericRatingDto
    {
        [JsonIgnore]
        public int BandId { get; set; }
        [JsonIgnore]
        public int? OrganizerId { get; set; }
        [JsonIgnore]
        public int? VisitorId { get; set; }
    }
}