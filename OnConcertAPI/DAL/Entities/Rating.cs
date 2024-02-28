using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Rating
    {
        public int Id { get; set; }
        [Required]
        public int Score { get; set; }
        public string Review { get; set; }
    }
}