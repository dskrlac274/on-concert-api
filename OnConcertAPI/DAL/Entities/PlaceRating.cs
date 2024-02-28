using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(RatingId))]
    [Index(nameof(BandId), nameof(VisitorId), nameof(PlaceId), IsUnique = true)]
    public class PlaceRating
    {
        [ForeignKey(nameof(Rating))]
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        [ForeignKey(nameof(Band))]
        public int? BandId { get; set; }
        public Band Band { get; set; }
        [ForeignKey(nameof(Visitor))]
        public int? VisitorId { get; set; }
        public Visitor Visitor { get; set; }
        [ForeignKey(nameof(Place))]
        public int PlaceId { get; set; }
        public Place Place { get; set; }
    }
}