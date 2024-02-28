namespace OnConcert.BL.Models.Dtos.Place.Rating
{
    public class DeletePlaceRatingDto
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public int VisitorId { get; set; }
        public int PlaceId { get; set; }
    }
}