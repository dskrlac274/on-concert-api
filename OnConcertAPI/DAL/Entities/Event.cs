using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OnConcert.DAL.Enums;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Event
    {
        public int Id { get; set; }
        [ForeignKey(nameof(PlaceId))]
        public Place Place { get; set; }
        public int PlaceId { get; set; }
        [ForeignKey(nameof(OrganizerId))]
        public Organizer Organizer { get; set; }
        public int OrganizerId { get; set; }
        [Column(TypeName = "nvarchar(30)")]
        public PlaceReservationStatus PlaceReservationStatus { get; set; } = PlaceReservationStatus.Requested;
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
        [Required]
        public DateTime DateFrom { get; set; }
        [Required]
        public DateTime DateTo { get; set; }
        public string Description { get; set; }
    }
}