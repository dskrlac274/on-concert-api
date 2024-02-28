using Microsoft.EntityFrameworkCore;
using OnConcert.DAL.Entities;

namespace OnConcert.DAL
{
    public class OnConcertContext : DbContext
    {
        public OnConcertContext(DbContextOptions<OnConcertContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Organizer> Organizer { get; set; }
        public DbSet<Band> Band { get; set; }
        public DbSet<Place> Place { get; set; }
        public DbSet<Visitor> Visitor { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<EventApplication> EventApplication { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<EventRating> EventRating { get; set; }
        public DbSet<PlaceRating> PlaceRating { get; set; }
        public DbSet<BandRating> BandRating { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}
