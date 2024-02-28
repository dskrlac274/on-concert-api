using OnConcert.BL.Models.Dtos.Event.Rating;

namespace OnConcert.BL.Models.Dtos.Place.Rating
{
    public class PlaceRatingResponseDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }
    }
}