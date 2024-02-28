using OnConcert.DAL;

namespace OnConcert.BL.Models.Dtos.Event
{
    public class GenericEventDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; } = DateTime.MinValue;
        public DateTime DateTo { get; set; } = DateTime.MinValue;
    }
}