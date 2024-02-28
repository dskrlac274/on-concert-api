using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Email), IsUnique = true)]
    public class Visitor
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string Email { get; set; }
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
    }
}
