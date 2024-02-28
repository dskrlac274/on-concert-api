using OnConcert.BL.Models.Enums;
using System.Text.Json.Serialization;

namespace OnConcert.BL.Models.Dtos.Event.Message
{
    public class EventMessageRequestDto : GenericMessageDetailsDto
    {
        [JsonIgnore]
        public int EventId { get; set; }
        [JsonIgnore]
        public int BandId { get; set; }
        [JsonIgnore]
        public int OrganizerId { get; set; }
        [JsonIgnore]
        public UserRole Role { get; set; }
    }
}