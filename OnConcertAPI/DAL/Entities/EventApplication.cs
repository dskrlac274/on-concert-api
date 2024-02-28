using Microsoft.EntityFrameworkCore;
using OnConcert.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class EventApplication
    {
        public int Id { get; set; }
        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }
        public int EventId { get; set; }
        [ForeignKey(nameof(VisitorId))]
        public Visitor Visitor { get; set; }
        public int? VisitorId { get; set; }
        [ForeignKey(nameof(BandId))]
        public Band Band { get; set; }
        public int? BandId { get; set; }
        [Column(TypeName = "nvarchar(30)")]
        public BandApplicationStatus? BandApplicationStatus { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}