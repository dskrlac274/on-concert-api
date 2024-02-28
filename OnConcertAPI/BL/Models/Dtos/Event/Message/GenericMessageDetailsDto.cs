namespace OnConcert.BL.Models.Dtos.Event.Message
{
    public class GenericMessageDetailsDto
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Data { get; set; } = string.Empty;
    }
}