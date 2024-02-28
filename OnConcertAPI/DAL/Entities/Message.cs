using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Message
    {
        public int Id { get; set; }
        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }
        public int EventId { get; set; }
        [ForeignKey(nameof(OrganizerId))]
        public User Organizer { get; set; }
        public int? OrganizerId { get; set; }
        [ForeignKey(nameof(BandId))]
        public Band Band { get; set; }
        public int? BandId { get; set; }
        [Required]
        public string Data { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}