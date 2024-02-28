namespace OnConcert.BL.Models.Dtos.Event.Application
{
    public class CreateEventApplicationDto
    {
        public int EventId { get; set; }
        public int? VisitorId { get; set; }
        public int? BandId { get; set; }
    }
}