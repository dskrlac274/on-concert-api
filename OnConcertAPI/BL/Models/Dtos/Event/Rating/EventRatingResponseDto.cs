namespace OnConcert.BL.Models.Dtos.Event.Rating
{
    public class EventRatingResponseDto : GenericRatingDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
    }
}