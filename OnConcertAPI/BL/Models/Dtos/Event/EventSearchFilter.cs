namespace OnConcert.BL.Models.Dtos.Event
{
    public class EventSearchFilter
    {
        public string Search { get; set; } = string.Empty;
        public bool? Upcoming { get; set; }
    }
}