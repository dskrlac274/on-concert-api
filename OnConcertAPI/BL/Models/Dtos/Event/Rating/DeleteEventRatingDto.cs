namespace OnConcert.BL.Models.Dtos.Event.Rating
{
    public class DeleteEventRatingDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int BandId { get; set; }
    }
}