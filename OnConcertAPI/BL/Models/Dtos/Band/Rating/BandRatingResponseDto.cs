using OnConcert.BL.Models.Dtos.Event.Rating;

namespace OnConcert.BL.Models.Dtos.Band.Rating
{
    public class BandRatingResponseDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int BandId { get; set; }
    }
}