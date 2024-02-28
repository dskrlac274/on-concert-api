using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Place
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(70)]
        public string Location { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}
