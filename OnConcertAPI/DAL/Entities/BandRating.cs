using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(RatingId))]
    [Index(nameof(BandId), nameof(OrganizerId), nameof(VisitorId), IsUnique = true)]
    public class BandRating
    {
        [ForeignKey(nameof(Rating))]
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        [ForeignKey(nameof(Band))]
        public int BandId { get; set; }
        public Band Band { get; set; }
        [ForeignKey(nameof(Organizer))]
        public int? OrganizerId { get; set; }
        public Organizer Organizer { get; set; }
        [ForeignKey(nameof(Visitor))]
        public int? VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
}