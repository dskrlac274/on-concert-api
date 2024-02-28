using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnConcert.DAL.Entities
{
    [PrimaryKey(nameof(Id))]
    public class Band
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int UserId { get; set; }
        public string Discography { get; set; }
    }
}
