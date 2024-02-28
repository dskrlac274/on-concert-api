using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Oib), IsUnique = true)]
    public class Organizer
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int UserId { get; set; }
        [Required]
        [MinLength(11)]
        [MaxLength(20)]
        public string Oib { get; set; } = string.Empty;
    }
}
