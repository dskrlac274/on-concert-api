using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(RatingId))]
    [Index(nameof(EventId), nameof(BandId), IsUnique = true)]
    public class EventRating
    {
        [ForeignKey(nameof(Rating))]
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public Event Event { get; set; }
        [ForeignKey(nameof(Band))]
        public int BandId { get; set; }
        public Band Band { get; set; }
    }
}