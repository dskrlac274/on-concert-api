namespace OnConcert.BL.Models.Dtos.Event.Rating
{
    public class GetEventRatingDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int BandId { get; set; }
    }
}
