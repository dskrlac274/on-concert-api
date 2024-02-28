using System.Text.Json.Serialization;
using OnConcert.DAL.Enums;

namespace OnConcert.BL.Models.Dtos.Event.Application
{
    public class EventApplicationResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int VisitorId { get; set; }
        public int BandId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BandApplicationStatus BandApplicationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}