using OnConcert.BL.Models.Dtos.Event.Rating;

namespace OnConcert.BL.Models.Dtos.Band.Rating
{
    public class GetBandRatingDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public int VisitorId { get; set; }
    }
}