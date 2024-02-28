using OnConcert.BL.Models.Dtos.Event.Rating;

namespace OnConcert.BL.Models.Dtos.Place.Rating
{
    public class GetPlaceRatingDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public int VisitorId { get; set; }
    }
}