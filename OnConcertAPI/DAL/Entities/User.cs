using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
        [Required]
        [MaxLength(70)]
        public string Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        [MaxLength(20)]
        public string ContactNumber { get; set; }
        [MaxLength(255)]
        public string WebUrl { get; set; }
        public string Description { get; set; }
    }
}