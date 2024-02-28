namespace OnConcert.BL.Models.Dtos.Event.Message
{
    public class EventMessageResponseDto : GenericMessageDetailsDto
    {
        public int? Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
    }
}